using System;
namespace EFTriggerHelper.Test
{
    public class PersonTbl 
    {
        public int PersonTblId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiyBy { get; set; }

    }

    public class LogTbl
    {
        public int LogTblId { get; set; }
        public string Description { get; set; }
    }
}
