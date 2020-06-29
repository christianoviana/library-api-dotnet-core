using Newtonsoft.Json;
using System;

namespace Library.API.Domain.Parameters
{
    public abstract class BasePaginationParams : ICloneable
    {
        private const int MAX_SIZE = 500;
        private const int MIN_SIZE = 1;

        public BasePaginationParams()
        {
            this.page = 1;
            this._size = 100;
        }

        private int _page;
        public int page
        {
            get
            {
                return this._page;
            }
            set
            {
                if (value <= 0)
                {
                    throw new Exception($"The page must be greater than zero");
                }
                this._page = value;
            }
        }

        private int _size;
        public int size
        {
            get {
                return this._size;
            }
            set {
                if (value > MAX_SIZE || value < MIN_SIZE)
                {
                    throw new Exception($"The max pagination size must be between {MIN_SIZE} to {MAX_SIZE}");
                }
                else
                {
                    this._size = value;
                }
            }
        }

        public object Clone()
        {          
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(jsonString, this.GetType());
        }    
    }
}
