<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> 7864dd8da4821481c77672150503091864b776b9

namespace PropertyManagementSystem.DAL.Entities
{
    public class SystemSetting
    {
        [Key]
        public int SettingId { get; set; }

        [Required, MaxLength(100)]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } // Email, Payment, General, AI, Security

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string DataType { get; set; } = "String"; // String, Int, Bool, JSON

        public bool IsPublic { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UpdatedByUser")]
        public int? UpdatedBy { get; set; }

        // Navigation
        public User UpdatedByUser { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
