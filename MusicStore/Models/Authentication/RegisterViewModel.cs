﻿//using MusicStore.Model.Enums;
using System.ComponentModel.DataAnnotations;
using MusicStore.Model.Enums;

namespace MusicStore.Models.Authentication
{
    public class RegisterViewModel


    {
        [Required(ErrorMessage = "Phone number can't be blank")]

        public string Phone { get; set; } = string.Empty;


        [Required(ErrorMessage = "First Name can't be blank ")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name can't be blank ")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email can't be blank ")]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank ")]
        [DataType(DataType.Password)]

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;


        //For userRoles
        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
    }

}
