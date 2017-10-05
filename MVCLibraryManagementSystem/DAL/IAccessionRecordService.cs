using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.DAL
{
    public interface IAccessionRecordService: IDisposable
    {
        IEnumerable<AccessionRecord> GetAllAccessionRecords();
        AccessionRecord GetAccessionRecordById(int? id);
        /// <summary>
        /// This method returns an ItemService instance for use in the Edit and Create views
        /// </summary>
        /// <returns>ItemService used by the class</returns>
        IItemService GetItemService();
        void Add(AccessionRecord r);
        void Update(AccessionRecord r);
        void Delete(int id);

        // IEnumerable<AccessionRecord> GetAccessionRecordsByItemId(int? id);
    }
}
