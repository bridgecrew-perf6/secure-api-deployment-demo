using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Data.Entities
{
    public class CompanyEntity
    {

        [Key]
        public Guid Id { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Address { get; set; }
        [MaxLength(150)]
        public string City { get; set; }
        [MaxLength(20)]
        public string Zip { get; set; }
        [MaxLength(150)]
        public string Country { get; set; }
        public bool IsEnabled { get; set; }
        public bool ReceivesNewsletter { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

    }
}
