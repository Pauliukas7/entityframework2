﻿public class Product : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    // Nav
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}