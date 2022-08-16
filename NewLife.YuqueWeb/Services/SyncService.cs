using NewLife.Data;
using NewLife.Log;
using NewLife.Threading;
using NewLife.YuqueWeb.Entity;

namespace NewLife.YuqueWeb.Services
{
    /// <summary>
    /// 同步服务
    /// </summary>
    public class SyncService : IHostedService
    {
        private readonly BookService _bookService;
        private readonly ITracer _tracer;
        TimerX _timer;
        TimerX _timer2;

        public SyncService(BookService bookService, ITracer tracer)
        {
            _bookService = bookService;
            _tracer = tracer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new TimerX(DoSyncBook, null, 1_000, 600_000) { Async = true };
            _timer2 = new TimerX(DoSyncDocument, null, 10_000, 600_000) { Async = true };

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.TryDispose();
            _timer2.TryDispose();

            return Task.CompletedTask;
        }

        async void DoSyncBook(Object state)
        {
            using var span = _tracer.NewSpan(nameof(DoSyncBook));
            try
            {
                var list = Book.GetValids();
                foreach (var item in list)
                {
                    if (item.Enable && item.Sync) await _bookService.Sync(item.Id);
                }
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                XTrace.WriteException(ex);
            }
        }

        async void DoSyncDocument(Object state)
        {
            using var span = _tracer.NewSpan(nameof(DoSyncDocument));
            try
            {
                // 只同步最近有改变的文章
                var start = DateTime.Now.AddDays(-1);
                var page = new PageParameter { PageSize = 100 };
                while (true)
                {
                    var list = Document.SearchByUpdateTime(start, DateTime.MinValue, page);
                    if (list.Count == 0) break;

                    foreach (var item in list)
                    {
                        await _bookService.Sync(item);
                    }

                    page.PageIndex++;
                }

                // 太久没同步的文章，都刷新一次
                var time = DateTime.Today.AddDays(2 - 1);
                page = new PageParameter { PageSize = 100 };
                while (true)
                {
                    var list = Document.SearchBySyncTime(time, page);
                    if (list.Count == 0) break;

                    foreach (var item in list)
                    {
                        await _bookService.Sync(item);
                    }

                    page.PageIndex++;
                }
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                XTrace.WriteException(ex);
            }
        }
    }
}