using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBot.Models
{
    public class ConfigModel
    {
        [Required(ErrorMessage = "Введите токен")]
        [Display(Name = "Токен")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Введите id группы")]
        [Display(Name = "Id группы")]
        public string GroupId { get; set; }

        [Required(ErrorMessage = "Введите id администраторов")]
        [Display(Name = "Id администраторов")]
        public string AdminIds { get; set; }

        [Required(ErrorMessage = "Введите id таблицы")]
        [Display(Name = "Id Google Таблицы")]
        public string SpreadsheetId { get; set; }
    }
}
