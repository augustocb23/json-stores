using System;
using System.Threading.Tasks;
using JsonStores.Sample.Models;

namespace JsonStores.Sample.CustomStores
{
    public class NoteStore : JsonRepository<Note, Guid>
    {
        // specify the file (ignore NamingStrategy option) 
        protected override string FileName => "notes";

        public NoteStore() : base(new JsonStoreOptions {Location = @"D:\notes"})
        {
        }

        public override Task AddAsync(Note obj)
        {
            obj.Id = Guid.NewGuid();
            obj.CreationDate = DateTime.Now;

            return base.AddAsync(obj);
        }

        public override Task UpdateAsync(Note item)
        {
            item.LastEdit = DateTime.Now;
            return base.UpdateAsync(item);
        }
    }
}