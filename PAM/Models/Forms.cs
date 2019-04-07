using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAM.Models
{
    // For our purpose we just need the content byte[], so technically we could just
    // put a byte[] inside Form and FilledForm. However, doing so would mean we have
    // to load the whole file into memory even when we just want to display the form
    // name. Because EF Core does not support lazy loading non-navigation properties,
    // we have to move the byte[] into its own class. The bright side of doing this
    // is that we can store more information about the uploaded file, and it gives us
    // the flexibility to store files on disk (instead of in database) if necessary.
    // BTW, it's probably not a bad idea to store the files on disk, but then it
    // requires additional configuration and separate data backup which we don't want
    // to bother.
    [Table("Files")]
    public class File
    {
        public int FileId { get; set; }

        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public byte[] Content { get; set; }
    }

    [Table("Forms")]
    public class Form
    {
        public int FormId { get; set; }

        [Required]
        public string Name { get; set; }

        public int DisplayOrder { get; set; } = 50;

        public bool ForEmployeeOnly { get; set; } = false;
        public bool ForContractorOnly { get; set; } = false;
        public bool Deleted { get; set; } = false;

        public List<SystemForm> Systems { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }
    }

    [Table("FilledForms")]
    public class FilledForm
    {
        public int FilledFormId { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }
    }

    [Table("SystemForms")]
    public class SystemForm
    {
        public SystemForm() { }

        public SystemForm(int systemId, int formId)
        {
            SystemId = systemId;
            FormId = formId;
        }

        public int SystemId { get; set; }
        public System System { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }
    }
}
