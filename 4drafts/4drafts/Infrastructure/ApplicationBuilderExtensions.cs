using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _4drafts.Data;
using _4drafts.Data.Models;
using System.Linq;
using System.Collections.Generic;

namespace _4drafts.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase
            (this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var data = scopedServices.ServiceProvider.GetService<_4draftsDbContext>();

            data.Database.Migrate();

            SeedCategories(data);

            return app;
        }

        private static void SeedCategories(_4draftsDbContext data)
        {
            if (data.Categories.Any()) return;
            var descriptions = new Dictionary<string, string>()
            {
                {"LF", "Literary fiction novels are considered works with artistic value and literary merit. They often include political criticism, social commentary, and reflections on humanity. Literary fiction novels are typically character-driven, as opposed to being plot-driven, and follow a character’s inner story." },
                {"Myst.", "Mystery novels, also called detective fiction, follow a detective solving a case from start to finish. They drop clues and slowly reveal information, turning the reader into a detective trying to solve the case, too. Mystery novels start with an exciting hook, keep readers interested with suspenseful pacing, and end with a satisfying conclusion that answers all of the reader’s outstanding questions." },
                {"Thri.", "Thriller novels are dark, mysterious, and suspenseful plot-driven stories. They very seldom include comedic elements, but what they lack in humor, they make up for in suspense. Thrillers keep readers on their toes and use plot twists, red herrings, and cliffhangers to keep them guessing until the end." },
                {"Horr.", "Horror novels are meant to scare, startle, shock, and even repulse readers. Generally focusing on themes of death, demons, evil spirits, and the afterlife, they prey on fears with scary beings like ghosts, vampires, werewolves, witches, and monsters. In horror fiction, plot and characters are tools used to elicit a terrifying sense of dread." },
                {"Hist.", "Historical fiction novels take place in the past. Written with a careful balance of research and creativity, they transport readers to another time and place—which can be real, imagined, or a combination of both. Many historical novels tell stories that involve actual historical figures or historical events within historical settings." },
                {"Rom.", "Romantic fiction centers around love stories between two people. They’re lighthearted, optimistic, and have an emotionally satisfying ending. Romance novels do contain conflict, but it doesn’t overshadow the romantic relationship, which always prevails in the end." },
                {"West.", "Western novels tell the stories of cowboys, settlers, and outlaws exploring the western frontier and taming the American Old West. They’re shaped specifically by their genre-specific elements and rely on them in ways that novels in other fiction genres don’t. Westerns aren’t as popular as they once were; the golden age of the genre coincided with the popularity of western films in the 1940s, ‘50s, and ‘60s." },
                {"Bild.", "Bildungsroman is a literary genre of stories about a character growing psychologically and morally from their youth into adulthood. Generally, they experience a profound emotional loss, set out on a journey, encounter conflict, and grow into a mature person by the end of the story. Literally translated, a bildungsroman is “a novel of education” or “a novel of formation.”" },
                {"Spec. Fic.", "Speculative fiction is a supergenre that encompasses a number of different types of fiction, from science fiction to fantasy to dystopian. The stories take place in a world different from our own. Speculative fiction knows no boundaries; there are no limits to what exists beyond the real world." },
                {"Scie. Fic.", "Sci-fi novels are speculative stories with imagined elements that don’t exist in the real world. Some are inspired by “hard” natural sciences like physics, chemistry, and astronomy; others are inspired by “soft” social sciences like psychology, anthropology, and sociology. Common elements of sci-fi novels include time travel, space exploration, and futuristic societies." },
                {"Fant.", "Fantasy novels are speculative fiction stories with imaginary characters set in imaginary universes. They’re inspired by mythology and folklore and often include elements of magic. The genre attracts both children and adults; well-known titles include Alice’s Adventures in Wonderland by Lewis Carroll and the Harry Potter series by J.K. Rowling." },
                {"Dyst.", "Dystopian novels are a genre of science fiction. They’re set in societies viewed as worse than the one in which we live. Dystopian fiction exists in contrast to utopian fiction, which is set in societies viewed as better than the one in which we live." },
                {"MR", "Magical realism novels depict the world truthfully, plus add magical elements. The fantastical elements aren’t viewed as odd or unique; they’re considered normal in the world in which the story takes place. The genre was born out of the realist art movement and is closely associated with Latin American authors." },
                {"RL", "Realist fiction novels are set in a time and place that could actually happen in the real world. They depict real people, places, and stories in order to be as truthful as possible. Realist works of fiction remain true to everyday life and abide by the laws of nature as we currently understand them." },
            };
            data.Categories.AddRange(new[]
            {
                new Category {Name = "Literary Fiction", Description = descriptions["LF"] },
                new Category {Name = "Mystery", Description = descriptions["Myst."]},
                new Category {Name = "Thriller", Description = descriptions["Thri."]},
                new Category {Name = "Horror", Description = descriptions["Horr."]},
                new Category {Name = "Historical", Description = descriptions["Hist."]},
                new Category {Name = "Romance", Description = descriptions["Rom."]},
                new Category {Name = "Western", Description = descriptions["West."]},
                new Category {Name = "Bildungsroman", Description = descriptions["Bild."]},
                new Category {Name = "Speculative Fiction", Description = descriptions["Spec. Fic."]},
                new Category {Name = "Science Fiction", Description = descriptions["Scie. Fic."]},
                new Category {Name = "Fantasy", Description = descriptions["Fant."]},
                new Category {Name = "Dystopian", Description = descriptions["Dyst."]},
                new Category {Name = "Magical Realism", Description = descriptions["MR"]},
                new Category {Name = "Realist Literature", Description = descriptions["RL"]},
            });

            data.SaveChanges();
        }
    }
}
