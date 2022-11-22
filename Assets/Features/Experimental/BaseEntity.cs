namespace Features.Experimental
{
    public class BaseEntity
    {
        public string Identity { get; }

        protected BaseEntity(string identity)
        {
            Identity = identity;
        }
    }
}
