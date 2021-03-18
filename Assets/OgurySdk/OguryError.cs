namespace OgurySdk
{
    public class OguryError
    {
        public OguryError(int errorCode, string description)
        {
            ErrorCode = errorCode;
            Description = description;
        }

        public int ErrorCode { get; }

        public string Description { get; }
    }
}