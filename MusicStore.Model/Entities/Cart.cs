using MusicStore.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CartId { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CartItem> CartItems { get; set; } = new(); 

    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; }
}