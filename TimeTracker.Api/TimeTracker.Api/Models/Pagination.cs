﻿using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Api.Models
{
    public record PaginationRequest
    {
        public PaginationRequest() { }

        [Range(1, int.MaxValue)]
        public int CurrentPage { get; set; } = 1;
        public int ElementsPerPage { get; set; } = 9;
    }

    public record PaginationResponseWrapper<T>
    {
        public int CurrentPage { get; set; }
        public int ElementsPerPage { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public List<T>? Results { get; set; }
    }

   
}
