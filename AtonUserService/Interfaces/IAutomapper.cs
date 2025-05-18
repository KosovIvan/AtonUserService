namespace TechProcessSupportSys.Interfaces
{
    public interface IAutomapper
    {
        public T Map<T, K>(K obj)
            where T : class, new()
            where K : class;
    }
}