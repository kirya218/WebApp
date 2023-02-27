using Omu.AwesomeMvc;

namespace WebApp.Tools.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInlColBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        ColumnModCfg AddInline(InlElm el);

        /// <summary>
        /// 
        /// </summary>
        Column Column { get; }

        /// <summary>
        /// add filter format
        /// </summary>
        /// <param name="el"></param>
        ColumnModCfg AddFilter(FilterElm el);
    }
}