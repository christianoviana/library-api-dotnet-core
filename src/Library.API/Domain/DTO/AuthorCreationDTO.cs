using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.API.Domain.DTO
{
    public class AuthorCreationDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
     
        [RequireWhenIdIsEmpty]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
      
        [RequireWhenIdIsEmpty]
        [JsonProperty("last_name")]
        public string LastName { get; set; }
      
        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }
    }

    public class RequireWhenIdIsEmpty: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var authorCreationDTO = (AuthorCreationDTO) validationContext.ObjectInstance;

            if (authorCreationDTO.Id != null && authorCreationDTO.Id != Guid.Empty)
                return ValidationResult.Success; 

            var _value = value as string;

            return string.IsNullOrWhiteSpace(_value)
                ? new ValidationResult($"The {validationContext.DisplayName} is required.")
                : ValidationResult.Success;            
        }
    }
}
