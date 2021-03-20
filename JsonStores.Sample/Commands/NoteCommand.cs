using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using JsonStores.Exceptions;
using JsonStores.Sample.CustomStores;
using JsonStores.Sample.Models;

namespace JsonStores.Sample.Commands
{
    public static class NoteCommand
    {
        public static void AddNoteCommand(this RootCommand rootCommand)
        {
            var command = new Command("note", "Add, remove or list notes");

            command.ListNotes();
            command.AddNote();
            command.EditNote();
            command.RemoveNote();

            rootCommand.Add(command);
        }

        private static void ListNotes(this Command command)
        {
            var c = new Command("list", "List all registered notes");
            c.AddAlias("ls");

            c.Handler = CommandHandler.Create(async (IConsole console) =>
            {
                var store = new NoteStore();
                var notes = await store.GetAllAsync();

                var view = new ListView<Note>(notes);
                if (view.IsEmpty) return;

                console.Append(view);
            });

            command.Add(c);
        }

        private static void AddNote(this Command command)
        {
            var c = new Command("add", "Adds a note")
            {
                new Argument<string>("description", "Description of the note")
            };

            c.Handler = CommandHandler.Create(async (string description) =>
            {
                var store = new NoteStore();
                try
                {
                    await store.AddAsync(new Note {Text = description});
                    await store.SaveChangesAsync();
                }
                catch (UniquenessConstraintViolationException e)
                {
                    ConsoleRendering.ClearLineAndWriteError(e.Message);
                }
            });

            command.Add(c);
        }

        private static void EditNote(this Command command)
        {
            var c = new Command("edit", "Edits the description for a note")
            {
                new Argument<Guid>("id", "Id for the note"),
                new Argument<string>("description", "New description for the note"),
            };

            c.Handler = CommandHandler.Create(async (Guid id, string description) =>
            {
                var store = new NoteStore();
                try
                {
                    var item = await store.GetByIdAsync(id);
                    item.Text = description;

                    await store.UpdateAsync(item);
                    await store.SaveChangesAsync();
                }
                catch (ItemNotFoundException e)
                {
                    ConsoleRendering.ClearLineAndWriteError(e.Message);
                }
            });

            command.Add(c);
        }

        private static void RemoveNote(this Command command)
        {
            var c = new Command("remove", "Removes a note")
            {
                new Argument<Guid>("id", "Id for the note")
            };
            c.AddAlias("rm");

            c.Handler = CommandHandler.Create(async (Guid id) =>
            {
                var store = new NoteStore();
                try
                {
                    await store.RemoveAsync(id);
                    await store.SaveChangesAsync();
                }
                catch (ItemNotFoundException e)
                {
                    ConsoleRendering.ClearLineAndWriteError(e.Message);
                }
            });

            command.Add(c);
        }
    }
}