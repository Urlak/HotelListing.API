namespace HotelListing.API.Models
{
    public class QueryParameters
    {
        public int StartIndex { get; set; }
        private int _pageSize = 15;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

    }
}
