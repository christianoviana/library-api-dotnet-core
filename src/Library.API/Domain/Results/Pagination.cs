using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Library.API.Domain.Results
{
    public class Pagination
    {
        public Pagination()
        {
        }
        
        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("has_next_page")]
        public bool HasNext
        {
            get
            {
                return this.CurrentPage < TotalPages;
            }
        }

        [JsonProperty("has_previous_page")]
        public bool HasPrevious
        {
            get
            {
                return this.CurrentPage > 1 && this.CurrentPage <= TotalPages;
            }
        }

        [JsonProperty("links")]
        public Links Links { get; set; }

        public void FormattLinks(HttpRequest request)
        {
            this.Links = new Links();

            UriBuilder builder = new UriBuilder(request.Scheme, request.Host.Host)
            {
                Path = request.Path
            };

            if (request.Host.Port.HasValue)
            {
                builder.Port = request.Host.Port.Value;
            }

           string url = string.Empty;
           var queryParams = QueryHelpers.ParseQuery(request.QueryString.Value);

            queryParams["page"] = "1";
            queryParams["size"] = this.PageSize.ToString();
            url = QueryHelpers.AddQueryString(builder.ToString(), queryParams.ToDictionary(k => k.Key, v => v.Value.ToString()));
            this.Links.FirstPage = new Uri(url);

            if (this.HasNext)
            {
                queryParams["page"] = (this.CurrentPage + 1).ToString();
                queryParams["size"] = this.PageSize.ToString();
                url = QueryHelpers.AddQueryString(builder.ToString(), queryParams.ToDictionary(k => k.Key, v => v.Value.ToString()));
                this.Links.NextPage = new Uri(url);
            }

            if (HasPrevious)
            {
                queryParams["page"] = (this.CurrentPage - 1).ToString();
                queryParams["size"] = this.PageSize.ToString();
                url = QueryHelpers.AddQueryString(builder.ToString(), queryParams.ToDictionary(k => k.Key, v => v.Value.ToString()));
                this.Links.PreviousPage = new Uri(url);
            }

            if (TotalPages > 1)
            {
                queryParams["page"] = this.TotalPages.ToString();
                queryParams["size"] = this.PageSize.ToString();
                url = QueryHelpers.AddQueryString(builder.ToString(), queryParams.ToDictionary(k => k.Key, v => v.Value.ToString()));
                this.Links.LastPage = new Uri(url);
            }        
        }
    }
}
