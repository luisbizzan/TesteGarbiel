namespace FWLog.Data.Repository.CommonCtx
{
    public abstract class BaseRepository
    {
        protected Entities Entities { get; set; }

        public BaseRepository(Entities entities)
        {
            Entities = entities;
        }
    }
}
