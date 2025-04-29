using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using MusicStore.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStore.Models
{
    public class AlbumListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public int TotalOrderedQuantity { get; set; }
        public int AlbumId { get; set; }

        [ForeignKey("Id")]
        public Album Album { get; set; }
    }

}