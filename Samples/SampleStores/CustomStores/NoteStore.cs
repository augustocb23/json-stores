using System;
using System.Threading.Tasks;
using JsonStores.Samples.SampleStores.Models;

namespace JsonStores.Samples.SampleStores.CustomStores
{
    public class NoteStore : JsonRepository<Note, Guid>
    {
        public NoteStore() : base(new JsonStoreOptions {Location = @"D:\notes"})
        {
        }

        // specify the file (ignore NamingStrategy option) 
        protected override string FileName => "notes";

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