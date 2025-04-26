using MusicStore.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int Id { get; set; }  // This matches the database column name

    public int Quantity { get; set; }

    [ForeignKey("Id")]
    public Album Album { get; set; }

    [ForeignKey("CartId")]
    public Cart Cart { get; set; }

    public DateTime AddedAt { get; set; }
}
