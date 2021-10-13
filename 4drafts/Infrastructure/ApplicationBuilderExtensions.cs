using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _4drafts.Data;
using _4drafts.Data.Models;
using System.Linq;
using System.Collections.Generic;
using System;

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

            SeedThreadTypes(data);
            SeedGenres(data);
            SeedUsers(data);
            SeedThreads(data);
            //SeedComments(data);

            return app;
        }

        private static void SeedThreadTypes(_4draftsDbContext data)
        {
            if (data.ThreadTypes.Any()) return;

            data.ThreadTypes.AddRange(new[]
            {
                new ThreadType { Name = "Story" },
                new ThreadType { Name = "Poem" },
            });

            data.SaveChanges();
        }

        private static void SeedGenres(_4draftsDbContext data)
        {
            if (data.Genres.Any()) return;
            var descriptions = new Dictionary<string, string>()
            {
                //STORY GENRES
                {"LF", "Literary fiction novels are considered works with artistic value and literary merit. They often include political criticism, social commentary, and reflections on humanity. Literary fiction novels are typically character-driven, as opposed to being plot-driven, and follow a character’s inner story." },
                {"Myst.", "Mystery novels, also called detective fiction, follow a detective solving a case from start to finish. They drop clues and slowly reveal information, turning the reader into a detective trying to solve the case, too. Mystery novels start with an exciting hook, keep readers interested with suspenseful pacing, and end with a satisfying conclusion that answers all of the reader’s outstanding questions." },
                {"Thri.", "Thriller novels are dark, mysterious, and suspenseful plot-driven stories. They very seldom include comedic elements, but what they lack in humor, they make up for in suspense. Thrillers keep readers on their toes and use plot twists, red herrings, and cliffhangers to keep them guessing until the end." },
                {"Horr.", "Horror novels are meant to scare, startle, shock, and even repulse readers. Generally focusing on themes of death, demons, evil spirits, and the afterlife, they prey on fears with scary beings like ghosts, vampires, werewolves, witches, and monsters. In horror fiction, plot and characters are tools used to elicit a terrifying sense of dread." },
                {"Hist.", "Historical fiction novels take place in the past. Written with a careful balance of research and creativity, they transport readers to another time and place—which can be real, imagined, or a combination of both. Many historical novels tell stories that involve actual historical figures or historical events within historical settings." },
                {"Roma.", "Romantic fiction centers around love stories between two people. They’re lighthearted, optimistic, and have an emotionally satisfying ending. Romance novels do contain conflict, but it doesn’t overshadow the romantic relationship, which always prevails in the end." },
                {"West.", "Western novels tell the stories of cowboys, settlers, and outlaws exploring the western frontier and taming the American Old West. They’re shaped specifically by their genre-specific elements and rely on them in ways that novels in other fiction genres don’t. Westerns aren’t as popular as they once were; the golden age of the genre coincided with the popularity of western films in the 1940s, ‘50s, and ‘60s." },
                {"Bild.", "Bildungsroman is a literary genre of stories about a character growing psychologically and morally from their youth into adulthood. Generally, they experience a profound emotional loss, set out on a journey, encounter conflict, and grow into a mature person by the end of the story. Literally translated, a bildungsroman is “a novel of education” or “a novel of formation.”" },
                {"Spec. Fic.", "Speculative fiction is a supergenre that encompasses a number of different types of fiction, from science fiction to fantasy to dystopian. The stories take place in a world different from our own. Speculative fiction knows no boundaries; there are no limits to what exists beyond the real world." },
                {"Scie. Fic.", "Sci-fi novels are speculative stories with imagined elements that don’t exist in the real world. Some are inspired by “hard” natural sciences like physics, chemistry, and astronomy; others are inspired by “soft” social sciences like psychology, anthropology, and sociology. Common elements of sci-fi novels include time travel, space exploration, and futuristic societies." },
                {"Fant.", "Fantasy novels are speculative fiction stories with imaginary characters set in imaginary universes. They’re inspired by mythology and folklore and often include elements of magic. The genre attracts both children and adults; well-known titles include Alice’s Adventures in Wonderland by Lewis Carroll and the Harry Potter series by J.K. Rowling." },
                {"Dyst.", "Dystopian novels are a genre of science fiction. They’re set in societies viewed as worse than the one in which we live. Dystopian fiction exists in contrast to utopian fiction, which is set in societies viewed as better than the one in which we live." },
                {"MR", "Magical realism novels depict the world truthfully, plus add magical elements. The fantastical elements aren’t viewed as odd or unique; they’re considered normal in the world in which the story takes place. The genre was born out of the realist art movement and is closely associated with Latin American authors." },
                {"RL", "Realist fiction novels are set in a time and place that could actually happen in the real world. They depict real people, places, and stories in order to be as truthful as possible. Realist works of fiction remain true to everyday life and abide by the laws of nature as we currently understand them." },
                //POEM GENRES
                {"Alleg.", "A narrative with two levels of meaning, one stated and one unstated." },
                {"Aub.", "A song or poem greeting the sunrise, traditionally a lover's lament that the night's passion must come to an end." },
                {"Ball.", "	Broadly speaking, the ballad is a genre of folk poetry, usually an orally transmitted narrative song. The term 'ballad' applies to several other kinds of poetry, including the English ballad stanza, which is a form often associated with the genre." },
                {"Blas.", "A Renaissance genre characterized by a short catalogue-style description, often of the female body." },
                {"Cen.", "A poem composed entirely of lines from other poems." },
                {"Dir.", "A funeral song." },
                {"DM.", "This might be called a 'closet soliloquy': a long poem spoken by a character who often unwittingly reveals his or her hidden desires and actions over the course of the poem. The 'I' of the dramatic monologue is very distinct from the 'I' of the poet's persona. Robert Browning was a master of this genre." },
                {"Eclo.", "A short pastoral poem; Virgil's eclogues are one of the first examples of this genre." },
                {"Ekph.", "Originally a description of any kind, 'ekphrasis' is now almost exclusively applied to the poetic description of a work of art." },
                {"Eleg.", "This genre can be difficult to define, as there are specific types of elegiac poem as well as a general elegiac mood, but almost all elegies mourn, and seek consolation for, a loss of some kind: the most common form of elegy is a lyric commemorating the death of a loved one. Greek elegiac meter, which is one source of what we know as the elegy today, is not normally associated with loss and mourning." },
                {"Ep.", "A long narrative poem that catalogues and celebrates heroic or historic deeds and events, usually focusing on a single heroic individual." },
                {"Epig.", "A brief and pithy aphoristic observation, often satirical." },
                {"Epit.", "A tombstone inscription. Several famous poems end with the poet writing his own. (See, for example, Thomas Gray's 'Elegy in a Country Churchyard' or W.B. Yeats's 'Under Ben Bulben.')" },
                {"Epithal.", "A song or poem that celebrates a wedding." },
                {"Fab.", "A brief tale about talking animals or objects, usually having a moral or pedagogical point, which is sometimes explicitly stated at the end. Aesop and la Fontaine are perhaps the most famous fable-writers." },
                {"Geo.", "The agricultural cousin of pastoral, a georgic is a poem that celebrates rustic labor." },
                {"Hym.", "A song of praise." },
                {"Inv.", "A personal, often abusive, denunciation." },
                {"Lam.", "An expression of grief." },
                {"LV.", "Poetry that is mostly for fun: this can mean anything from nonsense verse to folk songs, but typically there is a comical element to light verse." },
                {"Lyr.", "This genre encompasses a large portion of the world's poetry; in general, lyrics are fairly brief poems that emphasize musical qualities." },
                {"Masq.", "Courtly drama characterized by elaborate costumes and dances, as well as audience participation." },
                {"OV.", "Poetry written with reference to a particular event." },
                {"Od.", "A long, serious meditation on an elevated subject, an ode can take one of three forms." },
                {"Pae.", "A song of joy or triumph." },
                {"Pali.", "A recantation or retraction, usually of an earlier poem." },
                {"Pane.", "Poem or song in praise of a particular individual or object." },
                {"Par.", "A comic imitation." },
                {"Past.", "Originally a poem that depicted an idealized singing competition between shepherds, 'pastoral' has come to denote almost anything to do with a rural setting, although it also refers to several specific categories of the genre. Associated genres of varying synonymity are idyll, bucolic, eclogue, andgeorgic." },
                {"Psa.", "A sacred song." },
                {"Rid.", "A puzzling question that relies on allegory or wordplay for its answer. Riddles are often short, and often include an answer to the question posed, albeit an unsatisfying one. The riddle of the Sphinx, which Oedipus solved, is a particularly famous example: 'what walks on four legs in the morning, two at midday, and three in the afternoon?'" },
                {"Rom.", "An adventure tale, usually set in a mythical or remote locale. Verse forms of the romance include the  Spanish ballad and  medieval or chivalric romance." },
                {"Sat.", "	Ridicule of some kind, usually passing moral judgment." },
                {"Trag.", "This genre originated in ancient Greek verse drama and received extended treatment in Aristotle's Poetics, which made the downfall of the main character one of the criteria for tragedy. The genre has since expanded to include almost anything pertaining to a downfall." },
                {"VE.", "A letter written in verse, usually taking as its subject either a philosophical or a romantic question." },
            };
            data.Genres.AddRange(new[]
            {
                //STORY GENRES
                new Genre {Name = "Literary Fiction", SimplifiedName = "L.F.", GenreTypeId = 1, Description = descriptions["LF"] },
                new Genre {Name = "Mystery", SimplifiedName = "Myst.", GenreTypeId = 1, Description = descriptions["Myst."]},
                new Genre {Name = "Thriller", SimplifiedName = "Thrill.", GenreTypeId = 1, Description = descriptions["Thri."]},
                new Genre {Name = "Horror", SimplifiedName = "Horr.", GenreTypeId = 1, Description = descriptions["Horr."]},
                new Genre {Name = "Historical", SimplifiedName = "Hist.", GenreTypeId = 1, Description = descriptions["Hist."]},
                new Genre {Name = "Romance", SimplifiedName = "Rom.", GenreTypeId = 1, Description = descriptions["Roma."]},
                new Genre {Name = "Western", SimplifiedName = "West.", GenreTypeId = 1, Description = descriptions["West."]},
                new Genre {Name = "Bildungsroman", SimplifiedName = "Bild.", GenreTypeId = 1, Description = descriptions["Bild."]},
                new Genre {Name = "Speculative Fiction", SimplifiedName = "Spec.F.", GenreTypeId = 1, Description = descriptions["Spec. Fic."]},
                new Genre {Name = "Science Fiction", SimplifiedName = "Scie.F.", GenreTypeId = 1, Description = descriptions["Scie. Fic."]},
                new Genre {Name = "Fantasy", SimplifiedName = "Fant.", GenreTypeId = 1, Description = descriptions["Fant."]},
                new Genre {Name = "Dystopian", SimplifiedName = "Dyst.", GenreTypeId = 1, Description = descriptions["Dyst."]},
                new Genre {Name = "Magical Realism", SimplifiedName = "M.R.", GenreTypeId = 1, Description = descriptions["MR"]},
                new Genre {Name = "Realist Literature", SimplifiedName = "R.L.", GenreTypeId = 1, Description = descriptions["RL"]},
                //POEM GENRES
                new Genre {Name = "Allegory", SimplifiedName = "Alleg.", GenreTypeId = 2, Description = descriptions["Alleg."]},
                new Genre {Name = "Aubade", SimplifiedName = "Aub.", GenreTypeId = 2, Description = descriptions["Aub."]},
                new Genre {Name = "Ballad", SimplifiedName = "Ball.", GenreTypeId = 2, Description = descriptions["Ball."]},
                new Genre {Name = "Blason", SimplifiedName = "Blas.", GenreTypeId = 2, Description = descriptions["Blas."]},
                new Genre {Name = "Cento", SimplifiedName = "Cen.", GenreTypeId = 2, Description = descriptions["Cen."]},
                new Genre {Name = "Dirge", SimplifiedName = "Dir.", GenreTypeId = 2, Description = descriptions["Dir."]},
                new Genre {Name = "Dramatic Monologue", SimplifiedName = "DM.", GenreTypeId = 2, Description = descriptions["DM."]},
                new Genre {Name = "Eclogue", SimplifiedName = "Eclo.", GenreTypeId = 2, Description = descriptions["Eclo."]},
                new Genre {Name = "Ekphrasis", SimplifiedName = "Ekph.", GenreTypeId = 2, Description = descriptions["Ekph."]},
                new Genre {Name = "Elegy", SimplifiedName = "Eleg.", GenreTypeId = 2, Description = descriptions["Eleg."]},
                new Genre {Name = "Epic", SimplifiedName = "Ep.", GenreTypeId = 2, Description = descriptions["Ep."]},
                new Genre {Name = "Epigram", SimplifiedName = "Epig.", GenreTypeId = 2, Description = descriptions["Epig."]},
                new Genre {Name = "Epitaph", SimplifiedName = "Epit.", GenreTypeId = 2, Description = descriptions["Epit."]},
                new Genre {Name = "Epithalamion", SimplifiedName = "Epithal.", GenreTypeId = 2, Description = descriptions["Epithal."]},
                new Genre {Name = "Fable", SimplifiedName = "Fab.", GenreTypeId = 2, Description = descriptions["Fab."]},
                new Genre {Name = "Georgic", SimplifiedName = "Geo.", GenreTypeId = 2, Description = descriptions["Geo."]},
                new Genre {Name = "Hymn", SimplifiedName = "Hym.", GenreTypeId = 2, Description = descriptions["Hym."]},
                new Genre {Name = "Invective", SimplifiedName = "Inv.", GenreTypeId = 2, Description = descriptions["Inv."]},
                new Genre {Name = "Lament", SimplifiedName = "Lam.", GenreTypeId = 2, Description = descriptions["Lam."]},
                new Genre {Name = "Light Verse", SimplifiedName = "LV.", GenreTypeId = 2, Description = descriptions["LV."]},
                new Genre {Name = "Lyric", SimplifiedName = "Lyr.", GenreTypeId = 2, Description = descriptions["Lyr."]},
                new Genre {Name = "Masque", SimplifiedName = "Masq.", GenreTypeId = 2, Description = descriptions["Masq."]},
                new Genre {Name = "Occasional Verse", SimplifiedName = "OV.", GenreTypeId = 2, Description = descriptions["OV."]},
                new Genre {Name = "Ode", SimplifiedName = "Od.", GenreTypeId = 2, Description = descriptions["Od."]},
                new Genre {Name = "Paean", SimplifiedName = "Pae.", GenreTypeId = 2, Description = descriptions["Pae."]},
                new Genre {Name = "Palinode", SimplifiedName = "Pali.", GenreTypeId = 2, Description = descriptions["Pali."]},
                new Genre {Name = "Panegyric", SimplifiedName = "Pane.", GenreTypeId = 2, Description = descriptions["Pane."]},
                new Genre {Name = "Parody", SimplifiedName = "Par.", GenreTypeId = 2, Description = descriptions["Par."]},
                new Genre {Name = "Pastoral", SimplifiedName = "Past.", GenreTypeId = 2, Description = descriptions["Past."]},
                new Genre {Name = "Psalm", SimplifiedName = "Psa.", GenreTypeId = 2, Description = descriptions["Psa."]},
                new Genre {Name = "Riddle", SimplifiedName = "Rid.", GenreTypeId = 2, Description = descriptions["Rid."]},
                new Genre {Name = "Romance", SimplifiedName = "Rom.", GenreTypeId = 2, Description = descriptions["Rom."]},
                new Genre {Name = "Satire", SimplifiedName = "Sat.", GenreTypeId = 2, Description = descriptions["Sat."]},
                new Genre {Name = "Tragedy", SimplifiedName = "Trag.", GenreTypeId = 2, Description = descriptions["Trag."]},
                new Genre {Name = "Verse Epistle", SimplifiedName = "VE.", GenreTypeId = 2, Description = descriptions["VE."]},
            });

            data.SaveChanges();
        }

        private static void SeedUsers(_4draftsDbContext data)
        {
            if (data.Users.Any(u => u.Id == "68091adf-6141-48d9-8374-4693f21c6882")) return;
            Random rnd = new Random();
            var users = new List<User>
            {
                new User{
                    Id = "6aaa7f52-73b9-4ea1-8899-efa54cac082e",
                    UserName = "Dave",
                    NormalizedUserName = "Dave".ToUpper(),
                    Email = "david88@gmail.com",
                    NormalizedEmail = "david88@gmail.com".ToUpper(),
                    FirstName = "David",
                    LastName = "Copperfield",
                    Points = rnd.Next(0, 1000),
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://i.redd.it/7qalrjf53th51.png",
                },
                new User{
                    Id = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    UserName = "LadyBug",
                    NormalizedUserName = "LadyBug".ToUpper(),
                    Email = "kristen@gmail.com",
                    NormalizedEmail = "kristen@gmail.com".ToUpper(),
                    FirstName = "Kristen",
                    LastName = "Seinfeld",
                    Points = rnd.Next(0, 1000),
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn.pixabay.com/photo/2021/08/07/07/37/woman-6527938_960_720.png",
                },
                new User{
                    Id = "f76ba675-da23-45ae-b351-7854af84d238",
                    UserName = "Randy",
                    NormalizedUserName = "Randy".ToUpper(),
                    Email = "clipper77@gmail.com",
                    NormalizedEmail = "clipper77@gmail.com".ToUpper(),
                    FirstName = "Ronald",
                    LastName = "Raegan",
                    Points = rnd.Next(0, 1000),
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn.pixabay.com/photo/2016/03/31/19/57/avatar-1295404_960_720.png",
                },
                new User{
                    Id = "742466ec-5456-4f41-8b02-5ca6c710fa76",
                    UserName = "Ratata",
                    NormalizedUserName = "Ratata".ToUpper(),
                    Email = "malibu@gmail.com",
                    NormalizedEmail = "malibu@gmail.com".ToUpper(),
                    RegisteredOn = RandomDayFunc(),
                    Points = rnd.Next(0, 1000),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn.pixabay.com/photo/2019/09/14/09/44/cat-4475583_960_720.png",
                },
                new User{
                    Id = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    UserName = "MagicalProwess",
                    NormalizedUserName = "MagicalProwess".ToUpper(),
                    Email = "zenmaster@gmail.com",
                    NormalizedEmail = "zenmaster@gmail.com".ToUpper(),
                    FirstName = "Kiki",
                    Points = rnd.Next(0, 1000),
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn.pixabay.com/photo/2020/02/19/12/25/pug-4862083_960_720.png",
                },
                new User{
                    Id = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    UserName = "Annie",
                    NormalizedUserName = "Annie".ToUpper(),
                    Email = "anastasia@gmail.com",
                    NormalizedEmail = "anastasia@gmail.com".ToUpper(),
                    FirstName = "Annie",
                    Points = rnd.Next(0, 1000),
                    LastName = "Ruok",
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn.pixabay.com/photo/2013/11/28/11/30/lips-220244_960_720.jpg",
                },
                new User{
                    Id = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    UserName = "Janitor",
                    NormalizedUserName = "Janitor".ToUpper(),
                    Email = "janjan@gmail.com",
                    NormalizedEmail = "janjan@gmail.com".ToUpper(),
                    FirstName = "Roberto",
                    Points = rnd.Next(0, 1000),
                    LastName = "Rodrigez",
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://static.vecteezy.com/system/resources/previews/001/952/720/non_2x/mexican-cartoon-man-design-vector.jpg",
                },
                new User{
                    Id = "68091adf-6141-48d9-8374-4693f21c6882",
                    UserName = "Orochimaru",
                    NormalizedUserName = "Orochimaru".ToUpper(),
                    Email = "snakeboy@gmail.com",
                    NormalizedEmail = "snakeboy@gmail.com".ToUpper(),
                    RegisteredOn = RandomDayFunc(),
                    Points = rnd.Next(0, 1000),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://www.looper.com/img/gallery/why-orochimarus-character-arc-in-naruto-makes-no-sense/intro-1614956886.jpg",
                },
                new User{
                    Id = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    UserName = "Zombie",
                    NormalizedUserName = "Zombie".ToUpper(),
                    Email = "zombie@gmail.com",
                    NormalizedEmail = "zombie@gmail.com".ToUpper(),
                    FirstName = "Zom",
                    LastName = "Bie",
                    Points = rnd.Next(0, 1000),
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn4.iconfinder.com/data/icons/avatars-xmas-giveaway/128/zombie_avatar_monster_dead-512.png",
                },
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Hoodini",
                    NormalizedUserName = "Hoodini".ToUpper(),
                    Email = "Hoodini@gmail.com".ToLower(),
                    NormalizedEmail = "zombie@gmail.com".ToUpper(),
                    FirstName = "Pendejo",
                    Youtube = "www.youtube.com",
                    Twitter = "www.twitter.com",
                    Points = rnd.Next(0, 1000),
                    Instagram = "www.instagram.com",
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://i.pinimg.com/originals/66/cd/ee/66cdee36e7c372b7d51212eb802634fb.jpg",
                },
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "QskoSeksa69",
                    NormalizedUserName = "QskoSeksa69".ToUpper(),
                    Email = "qskoseksa@gmail.com",
                    NormalizedEmail = "qskoseksa@gmail.com".ToUpper(),
                    FirstName = "Poopie",
                    LastName = "Pants",
                    Youtube = "www.youtube.com",
                    Twitter = "www.twitter.com",
                    Points = rnd.Next(0, 1000),
                    Instagram = "www.instagram.com",
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://i.dailymail.co.uk/i/pix/2015/01/20/24DE2A9B00000578-0-image-a-1_1421752994677.jpg",
                },
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Stoqn_Kolev",
                    NormalizedUserName = "Stoqn_Kolev".ToUpper(),
                    Email = "stoikata@gmail.com",
                    NormalizedEmail = "stoikata@gmail.com".ToUpper(),
                    FirstName = "Stoqn",
                    LastName = "Kolev",
                    Youtube = "www.youtube.com",
                    Twitter = "www.twitter.com",
                    Points = rnd.Next(0, 1000),
                    Instagram = "www.instagram.com",
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://pbs.twimg.com/profile_images/1245854439975550976/IsJ1Baek_400x400.jpg",
                },
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "MajorTortoise",
                    NormalizedUserName = "MajorTortoise".ToUpper(),
                    Email = "emailatemail@gmail.com",
                    NormalizedEmail = "emailatemail@gmail.com".ToUpper(),
                    FirstName = "David",
                    LastName = "Spade",
                    Youtube = "www.youtube.com",
                    Facebook = "www.facebook.com",
                    Points = rnd.Next(0, 1000),
                    Instagram = "www.instagram.com",
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://i.pinimg.com/736x/e5/6f/0e/e56f0ef1ed61bf011aab7e11956666e4.jpg",
                },
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "derp",
                    NormalizedUserName = "derp".ToUpper(),
                    Email = "derp@gmail.com",
                    NormalizedEmail = "derp@gmail.com".ToUpper(),
                    Facebook = "www.facebook.com",
                    Website = "www.website.com",
                    RegisteredOn = RandomDayFunc(),
                    Points = rnd.Next(0, 1000),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://assets.thehansindia.com/h-upload/2020/06/10/975863-image-3.jpg",
                },
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Why_Tho",
                    NormalizedUserName = "Why_Tho".ToUpper(),
                    Email = "ytho@gmail.com",
                    NormalizedEmail = "ytho@gmail.com".ToUpper(),
                    FirstName = "y",
                    LastName = "thoo",
                    Youtube = "www.youtube.com",
                    Twitter = "www.twitter.com",
                    Instagram = "www.instagram.com",
                    RegisteredOn = RandomDayFunc(),
                    Points = rnd.Next(0, 1000),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://en.meming.world/images/en/thumb/e/e2/Crying_Cat_screaming.jpg/300px-Crying_Cat_screaming.jpg",
                },
            };

            data.Users.AddRange(users);
            data.SaveChanges();
        }

        private static void SeedThreads(_4draftsDbContext data)
        {
            if (data.Threads.Any(t => t.Id == "8725ba9f-490d-460a-b6cf-d19d9c2ecb37")) return;
            Random rnd = new Random();

            var stories = new List<Thread>
            {
                new Thread
                {
                    Id = "fe016357-389e-4d3f-b335-1e3a9f17ffb3",
                    ThreadTypeId = 1,
                    Title = "The Stalker",
                    Content = @"
It was a typical fall morning- I sought out my usual seat at the local bistro down the street; the previous day had been an excruciatingly long one, and the one thing that got me through was knowing I always had my solo-breakfast date in the morning.

I sat down on the familiar comfort of the wicker chair, basking in the morning light streaming through the large glass window.

I opened my laptop with to check the local news- I was attempting to act more “adult like”, and adults do that, right?

I opened the browser, and was taken aback by the big, bold letters on the headline: THIRD WOMAN FOUND DROWNED; A SERIAL KILLER IS ARISING. I involuntarily gasped so loud that the elderly man sitting in my peripherals gave a startled jump. In the description, it was said that the women were all in their early 20’s, with no true connection with one another; the only evidence tying them together is that they all had locks of hair missing upon investigation.

Suddenly feeling uneasy, I left money on the table to cover my untouched breakfast, sent a quick email to work explaining that I needed a sick day, and hastily grabbed my belongings. I started walking down the familiar cobblestone road, journeying back home. Pepper spray clasped in one hand, my keys grasped in the other; I felt paranoid, and my heart was exploding with adrenaline. I kept believing I was being followed, turning only to see a bare dry leaf skidding across the pavement. Before I had time to even shift my view back to ahead of me, I bumped straight into a tall, muscular figure that towered over me. My fight or flight kicked in, until I looked up to see kind eyes and an apologetic smirk. He asked if he could escort me home, given the recent danger that over compassed the town. As we arrived to my townhouse, I learned that his name was John; and that he would be accompanying me to breakfast the next morning.

The first few months, we were blissful in the throws of young love. The honeymoon phase never faltered, the love grew stronger every minute. And then suddenly, when I expressed my undying love and proposed moving in with him, John grew distant. The kindness in his eyes had been replaced with worry and despair. He officially broke things off with me a week later.

I decided to start following him home. “I’m not being predatory”, I told myself, “I’m just making sure he’s doing alright”. As I drove by, I saw what was my deepest and most dreading thought I had; he was opening the passenger side door of his car for a woman; long, naturally wavy red hair, her thin waist that accentuated her beautiful curved hips, and a smile that could light up the darkest room. My heart immediately burst into flames, my eyes immediately welling, streaming salty tears into my gaping mouth, and my hands grasping the steering wheel so tight, I thought my circulation was cutting off. Not wanting to cause any attention to myself, I drove home, every emotion filling my entire being to the core.

As I got home, I opened a bottle of wine that had been collecting dust (I hardly ever drank), and let my dying insides feel the warmth and comfort of the alcohol coursing through me. After the first bottle came a second, and after the second came a third. I must have really blacked out, because when I awoke, I was sitting at the edge of a canal not too far from home. I tried to search my mind of any evidence of how I got there; until I saw a figure approach in the darkness. As they got closer and closer, I blacked out again.

I awoke to sirens and lights and several EMT’s surrounding me, all seemingly relieved that I had come to. When I asked what had happened, they explained that they found me washed up on shore. It had looked like I had a wicked fight and battle I had endured, and they were shocked I made it out alive. Once the police wrapped me in a blanket and finished questioning me, I exhaled a sigh of relief. I knew my plan would have been foolproof, and that staging myself as the victim would push myself further down the suspect list. I knew the redhead would meet me there, as long as I posed as John, sending a brief text. One thing I didn’t mention that was in the headlines?

Every woman that had gone missing was one who had taken one of my lovers away. Suspicion started to rise as they started to bring me in for questioning more and more; but I knew at least now I was safe, as long I tended to the body shortly. As the officer took one final glance back at me, I kept on the mask of confusion, sorrow, and fear. When he turned his back, I allowed myself to smile, as I reached into my pocket and brushed my fingers against my new trinket- a lock of wavy red hair.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "972026cd-3505-4f04-9ac7-382c6541ba70",
                    ThreadTypeId = 1,
                    Title = "Instant Messaging",
                    Content = @"
It all started on the 14th night of march, the night of my parents’ 20th wedding anniversary.

It was a wonderful, sunny day, if memory serves. Surprisingly warm for before the beginning of spring. The beautiful weather was perfect for the atmosphere of the day—being married for twenty years is obviously a momentous occasion, so my parents had booked a table at our favorite Italian restaurant. 

Of course, this was a formal occasion, so I had my best suit on. It was 5:33, and I was just straightening my tie when my phone went off—I’d received a message. That’s strange, I thought, that never happens. I checked the message: It was from my mum. It was quite a jumble of numbers and letters, but through the vocabulary stew I could make out the legible phrase: “Please help me.” It should go without saying that this worried me greatly, so I immediately replied, “Are you okay?” Just as instantly, I got another text which read, “Oops. Pocket text!” I signed with all the relief I had and continued to prepare myself. 

A few minutes later, I received yet another message, this time from my dad. I checked the text, and once again it was a massive mixture of letters and numbers, with the phrase, “Please help me” concealed within. Creepy though this was, my dad was always a joker, so I presumed he was just joking around, until I was sent another text saying, “Oops. Pocket text!” Now this sparked panic. Pure, unmistakeable panic. Exactly half a minute passed when I received the exact same message from my sister. This could not be coincidental. It just couldn’t. 

In a state of sheer anxiety, I started to run to the restaurant. I made it about a quarter of the way before I was stopped by a police officer. “Main road’s closed,” he said, “Huge car crash.” This was the exact moment I realized just what had happened. I demanded to see the wreckage, a request I’m surprised was allowed. When I got there, it wasn’t the remnants of the car that caught my eye, not the flames billowing from the destroyed vehicle. No, I was horrified to see the lifeless corpses of my mother, father and sister. I asked for the estimated time of their deaths—all three of them were killed instantly by the collision at 5:32.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "937e8c3d-5474-4e32-b019-cbe628bba891",
                    ThreadTypeId = 1,
                    Title = "Darkness in the Rearview Mirror",
                    Content = @"
In the summer of 2013, I found myself driving home alone on highway 902 from a party. It was almost midnight, and needless to say it was pitch black. As was usual at night, I was on edge. I had the radio off, and could hear nothing but the muffle roar of tires on pavement and the dull hum of the engine. I stole a glance into the middle rear view mirror, and saw nothing but darkness through the back window. 

I know that I looked backward and saw nothing. I’m sure of it. Just the seemingly endless blackness of the night. I remember it so clearly because not 10 seconds later a car passed me to the left. Headlights on. I had one of those sudden adrenaline rushes like when you think you see a person outside your bedroom window when it’s just a tree, or when you start awake at night with the feeling of falling. Ten seconds earlier, nothing had been behind me. Suddenly, a car. I drove the rest of the way home shivering and knowing something was off.  

The next morning, I found two sets of scratches near the back of my van. One was on the left rear, one was on the right. The car was pretty old. They could have been there for months, but that was the first time that I distinctly remembered seeing them. 

In hindsight, there are two possibilities for what happened that night. Possibility one. By some glitch in reality, or something paranormal, this other car had somehow appeared behind me within 10 seconds of me checking my mirror. Like some weird ghost crap or something. However, the second option is what makes my blood run cold whenever I consider it. 

It didn’t even occur to me until months after the fact, but it makes me dread driving alone at night even more. Possibility two. The car was normal. It had approached me from the rear and passed me to my left. However, something large, and wide, and as black as the night had been clinging to the rear of my car, obscuring my view through the window and leaving deep scratches on the sides. 

And I had inadvertently driven it home with me.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e78055f5-d6da-4ab4-a05d-000a3c5c5281",
                    ThreadTypeId = 1,
                    Title = "Roommate Troubles",
                    Content = @"
This actually happened to me a few years back at the University of the Arts in Philadelphia. 

My sophomore year, I roomed with a girl named Kara. She was a jazz vocalist, but her main interest was opera. We had a small room on the sixth floor of a dormitory called Juniper Hall. The walls were thin, and her last night singing and voice practices would keep me up late. After a month or so of lost sleep, I convinced her to move her last night practices to the music studios in the Merriam theater building a block away. 

Around 8:00 one evening, Kara announced that she would be practicing late for an upcoming recital and probably wouldn’t be home until around midnight. Great, I thought, that means I can go to bed early (I was beat … I had a horrible day in acting studio, and was ready to pass out as soon as I had dinner). She said goodnight and left, coffee and sheet music in hand. 

I made some grilled cheese and soup, gobbled it down, and immediately began to prepare for bed. By the time I got out of the shower, my eyelids were so heavy I could hardly brush my teeth. I pulled on my PJ’s and crawled into the top bunk of our bunk bed. I was out as soon as my head hit the pillow. 

I should take a second to describe the layout of our apartment. When entering the apartment, the bedroom was through a door immediately to the left. Our bathroom was inside the bedroom, just past the bunk beds. (UArts is nice in the sense that you don’t have to share bathrooms).

Anyway, I woke up to the sound of the apartment door closing. I opened my eyes and groggily check my phone: midnight on the dot. I rolled back over and closed my eyes. I heard Kara enter the room and stop in front of the bunk bed. Checking to see if I’m actually asleep, i thought. She flopped down on the bed below me, which was strange, as she was a stickler for brushing her teeth and washing up before bed. Then again, exams were just around the corner, and we were alle exhausted. The mattress below me creaked, and then was silent. I couldn’t even hear her breathing. 

I started to drift off again. I was just on the edge of deep sleep when I was startled awake again by a noise. 

A key in the lock. The door opening. 

And Kara entering our apartment, humming an opera tune. 

The mattress below me creaked.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "aa5067eb-6029-48f3-968a-6ab92d66b953",
                    ThreadTypeId = 1,
                    Title = "Kids in the Dark",
                    Content = @"
Growing up poor in the Deep South meant sharing a lot with my little brother, Ollie. Most often, we'd pass toys, clothes, and skin conditions between us. Up until he was six, we even shared a bed. Neither of us was happy about that.

It was my 10th birthday when that changed. I got one present that year, and it was a bed of my own. Ollie was jealous right away, and I could understand why. He had to keep that half-broken down frame with the worn out mattress. The one I'd gotten wasn't much better, but not being broken and worn was enough.

Sleeping apart was a great feeling. It was freedom. No longer would I have to suffer the sudden and inexplicable kicks to the stomach. No longer would I wake up with Ollie's foot pressed into my neck like he'd stepped on Dracula the night before.

At least, that's what I'd thought.

Right away, right after I got the new bed, the shriek started.

At first I thought Ollie woke up in the middle of the night and screamed because he'd gotten scared. Then, the sound echoed through the tiny room again and I knew it wasn't a normal cry.

The room was always black as pitch after sunset. The one window we had was pressed against a long leaf pine and even the biggest, brightest moon cast no light inside.

The shriek just about drove me crazy. Every night, probably at the same exact time, these sharp yelps would knock me right out of my dreams. It wasn't my Mom or Dad yelling, either. I knew what that sounded like, believe me! Most worrying of all was the fact I could never tell where it was coming from. It seemed completely random.

One night it'd come from somewhere near the closet. The next, it'd shoot out from a corner of the ceiling.

Any hope I'd had of having my own space would get dashed every time as Ollie would silently slip into the bed with me, shaking like crazy. He'd clasp onto me and wouldn't let go until it was almost daybreak. Most times I'd take his hand and tell him everything was going to be okay, that it'd be over by morning ... but I was never really sure.

Over time, the shriek started changing. At first it was only by small degrees, but eventually it took on the primal hooting sound of a primate calling out its fierce warning. I had to clasp pillows to my ears just to keep from going deaf.

Mom and Dad never believed me or Ollie, basically because the thing ... whatever it was ... refused to make a peep when they were in the room. Apparently they couldn't even hear it through the walls even though it was damned sure loud enough!

The shriek just got worse and worse until I felt like I couldn't take it anymore. Me and Ollie were doing really bad in school, and we just had no energy at all. I could sleep more deeply with my head propped up and eyes open in the middle of class than in my own room at night.

Then, thankfully, we moved out of the house nearly a year later. I had contemplated all sorts of things, even a child's clumsy concept of suicide, to get away from the horrific nightly noise.

There was no problem at the next house. It was a nice white cookie-cutter home on a dead end street, and I welcomed the normalcy. What's more, when we moved in there was a bunk bed waiting for me and Ollie. No more broken bed, no more second bed I ended up having to share anyway.

The only problem was deciding who'd get the top bunk.

I told Ollie I deserved it. After all, I had gotten a new bed way back, and he ruined it by climbing in every night.

""What?"" He shook his head, ""I never did that."" 

I had always wondered why the noise stopped the second I was sharing my bed. Now I had the answer.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "df049477-da1e-4a0c-a779-729db036e4d9",
                    ThreadTypeId = 1,
                    Title = "My sister was a sociopath. Then she had surgery.",
                    Content = @"
There was always something wrong with Annie. For years, it felt like I was the only one who knew.

When we were kids, we used to see our little cousins quite often. Our house, their house. My mom and aunt drank wine and bonded over having lost their husbands, my uncle in the grave and my dad, in jail. Annie and I were much older than the other kids, but I’d still hang out with them, just to be safe and keep an eye on my sister. If I left her alone with them, someone would wind up hurt. One time, she’d stuck a clothespin on their cat and watched it run circles around the room. She was twelve. Another time, she’d pressured our youngest cousin to drop that same cat out a third floor window, mocking him for not wanting to do it. “I can’t believe you’re actually scared,” I’d heard her say. By the time I got up there, my little cousin had let go. The cat was fine, thank god. But my cousin was not. He was traumatized, screaming and crying behind his bedroom door. Annie told Mom that she was really sorry and that she’d learned in school that cats could survive such falls. It was all bullshit, Annie had never felt sorry a day in her life. But Mom ate it up every time, because Annie was her special little girl.

After Dad went away, our grandfather came over a lot to help Mom out. Her dad, as we hardly knew my father’s parents. I was very close with my Papa. He was probably the person I looked up to most. The man was never in a bad mood. At least if he was, he never showed it. He brought something to that house that had long been missing. Music, dancing, laughter. He’d teach me things my dad never did, like how to ride a bike, or tie a tie. Or, when Mom wasn’t home, how to use the power tools Dad left dusty in the basement. It didn’t matter what we did. There was comfort in simply having him there, waking up every day to find him already sitting at the kitchen table reading the paper, only to drop it straight away so he could cook me something for breakfast. Papa loved watching me eat, almost as much as he loved telling stories. He’d given me this small military medal once and told me about how he’d almost died earning it. Said he wasn’t much older than me when he got it. It didn’t feel right to keep it, but he was happy to pass it down, and even happier when he saw it pinned to my backpack the next day.

“Now you can take me with you when I’m in the ground,” he laughed. He joked, but he knew. Knew that I’d need his guidance even in death. Papa may have been a jolly, old Italian man, but he was sharper than he looked. He knew something was very wrong with his granddaughter, and knew that once he was gone, things were only going to get harder for all of us. Annie did nothing to hide her contempt for the relationship I had with Papa. She’d always looked on with a scowl. When Papa passed, she’d come into my room with bright eyes and said, “Are you sad Papa’s dead?” I screamed and told Mom but Annie pretended to be an ignorant child, and my mother was in no place to deal with it. During the services, Annie watched me like entertainment. I tried my hardest to hold everything in, to not give her any satisfaction. And though it did simmer her attention, it only heightened everyone else’s. People were apparently asking my mother what was wrong with me. The fact that I was looked upon with such scrutiny while Annie went unnoticed drove me insane, especially since the loss of my grandfather hurt me more than anything. And when his medal fell off my backpack the following week, it crushed me further. I came home from school in tears, totally inconsolable despite my mother’s attempts. Annie just sat there, looking amused. “Who’s gonna watch over you now?” she’d asked. I shoved her hard and Mom grounded me.

I thought about killing her that night.

The affect Annie had on me extended even beyond her reach. There was this ever-present mistrust in my mind, this cancerous red-flag that always waved. I’d spent my whole life watching my sister pretend to be something she’s not, to the point that even the most innocuously feigned interaction turned me off. Like when a cashier asks you how you are doing and you say ‘Good’ and ask them back. But you don’t care. They don’t care. I worried that this was true for everyone, always. So I kept to myself and never made very many friends.

Annie’s reign of terror continued on into high school. I got to spend one year there without her and it was the best year of my life. I actually couldn’t wait to go to school. Then she was a freshman, and I was back to spending afternoons in the counselor’s office. I never said much, and so Mr. Wyle treated me like every other anxiety-ridden student, offering me numerous breaks and check-ins. I didn’t know how to tell him that I was terrified of my fourteen year-old little sister, the sweet young girl that everyone was just now meeting. It hadn’t taken her long to adapt to her new environment. She threw on that sheep’s clothing and did what she does best: hide, and hurt. She was smart about it, much smarter than when she was a kid. It was always just painful enough to scar her victims, but simple enough to be overlooked by the rest of us. She’d date boys and break their hearts, just to take them back and break up all over again. It looked like casual teenage drama, but I knew she was doing it for fun. She’d toe the line with her male teachers, keep her best friend feeling like shit about herself, and tell her other friends that I was abusive toward her. I fucking hated it, and hated more so the fact that I had to let her get away with it. If I pushed, she’d push harder. I had to keep myself out of her mind.

Still, the thought of that stupid smirk as she soaked in the pain she’d caused made me see red.

Then I met Ms. Harden, the school’s new counselor. She’d seen how often I visited the reset-room in the past and wanted to get to know me. I wasn’t so receptive at first, but Harden never gave up on me. For weeks, I’d meet with her and in time I’d opened up. She seemed different. She didn’t talk to me from any position of authority, or with condescension. It felt like the person she was inside that room was the same person outside of it, which meant more to me than she knew. My red flags went down, as they rarely had. So when she asked me one day what I was afraid of, I told her everything. Harden was intrigued, so I kept going. It all came spilling out of me and I couldn’t stop. The release gave me relief I had never felt before.

Until Annie confronted me at my locker. “What did you say to her?” Harden had asked to meet with her, and she was livid. I couldn’t look her in the eye, my five-foot freshman of a little sister, so I dug around my locker like I was looking for something.

“Nothing,” I replied. I continued rummaging in hopes that she’d go away, or that somebody else would come talk to us. But nobody around us paid us any mind. Hell, it might have even looked like a sweet moment between brother and sister. Then Annie slammed the locker onto my hand. I howled and cursed loud enough to freeze the entire corridor. Teachers came running out of their classrooms as students buzzed with confusion, while those closer to me gasped and cried for help. I slid down to the floor and crunched into a tight ball, holding my hand to my chest, afraid to look at it. Annie had already disappeared.

I was lucky to have escaped with no worse than a bruise on the top of my hand. It hurt to make a fist, but it was better than a severed finger. Of course, Annie got in trouble with the school, and Mom. But what seemed to have bothered her most was the unraveling of the character she’d played for everyone. People were now talking, noticing things she never wanted them to notice, seeing her in a light she’d never wanted cast upon her. One of the upperclassmen called her a “little ginger snap”, and it caught on. She fucking hated that. And it was only going to get worse. Harden was now looking to meet with Annie regularly, and Annie would soon discover that her usual tricks were no match for a trained professional. Someone was finally seeing through the feigned innocence, the tales of grandeur, the timely sob stories. Thus began the chess match. When Annie skipped on her meeting with Harden, Harden called home. When Mom scheduled a joint meeting, Annie ate soap in the bathroom and made herself throw up. I was curious to see how long this battle would last, you just couldn’t underestimate how far Annie was willing to go. But I think she was smart enough to realize that any further resistance was just further evidence against her. I reveled in her misery the day she finally gave in. It wasn’t long before Harden suggested my mother take Annie to a psychologist. She explained to her how her daughter showed worrying signs of an anti-social personality. As ignorant and naïve as my mother had always been, it was now undeniable: Annie was a real life, near-diagnosable, manipulative little sociopath.

Poor Mom was beside herself. She cried and cried while pacing the kitchen with a cigarette in her shaking hand. She was at a loss, so she did exactly what was recommended of her. Annie was to be seeing the psychologist every week. Sometimes, Mom and I would join her. I had to hold in my excitement over seeing Annie so uncomfortably vulnerable, the way she’d always made everyone else feel. She’d stare daggers at me during the sessions. I’d try my best to appear neutral, to be like her and not show any emotion or fear whatsoever, but it wasn’t easy, not even after the fake apology she gave me. She spoke no truth in those sessions. Blamed her behavior on the absence of our father. Mom and the doctor deemed it progress, but not me. And Annie knew. Every time we got home, she’d shoot me this piercing glance before locking herself away in her room for the night, and only then could I finally breathe, though not for very long. I’d started sleeping with a damn knife under my pillow, just in case. If I started to feel ridiculous for doing so, I’d remind myself not to underestimate how far this girl was willing to go to get what she wanted. And right now, it felt like she wanted me dead.

A few weeks passed. It was hard to tell if the behavior therapy was having any real affect on Annie. The psychologist assured my mother to give it more time, but Mom was hysterical and impatient. So she did the worst thing anyone could do: she went online. She was up all night reading whatever bullshit she could find. From dietary treatment of personality disorders (“Buy our special product!”), to early signs that your child is a serial killer. It was fucking crazy, and it made my mother even crazier. That was when she found Dr. McKinnon. He ran some small, private practice down in Boston, a few hours south of us. His website touted him as an expert in psychology, with particular emphasis on treatment of personality disorders. There was also a link to a news article about the work he’d done with the FBI in catching the Bear River Killer, who he’d gone on to establish a relationship with in order to write the book he’d made sure to advertise on the website. Mom wrote to Dr. McKinnon, and he responded almost immediately, promising that he could help with our situation. This man claimed to have invented a device that could alter the pathways in Annie’s brain that made her the way she was, and rewire them to function normally. For a hefty fee, of course. Crazed and desperate, Mom didn’t hesitate. Drove down that weekend, signed every waver they threw at her, and scheduled surgery for the day after school broke for the summer, just six weeks out. Even booked a hotel room for the few days Annie would be spending in recovery. I thought she was out of her mind for this, and even more so for believing Annie would just allow it to happen. They’d had a blowout when Mom told her what she’d done.

“Why would you do this to me?” Annie kept saying. “You think there’s something wrong with me?”

“Yes, Annie! Yes!”

It hurt my mother to say this, and hurt even more when Annie said, “You raised me. I’m your daughter.” She knew this was the very thing that would hurt Mom the most.

“I didn’t raise you to act like this!” Mom shouted, tears in her eyes.

Annie ignored her. “I wanna go to another school.”

“What? Why? What’s wrong with your school?”

“Everyone thinks I’m crazy. Send me to St. John’s.”

Mom huffed. “I don’t have the money for that, Annie!”

“Cancel the surgery.”

“It’s either the surgery or I’ll have you committed,” Mom snapped. “Which one?”

That shut Annie up faster than I’d ever seen, and off she went to her room. When she was gone, Mom released the sob she’d been holding in as I awkwardly sat across the room, having just witnessed the whole thing. I felt bad, but was glad to see her stand her ground. Although I half expected Annie to run away that night. Or worse. Ended up barricading my bedroom door and kept a grip around the knife under my pillow as I slept.

But the days passed without incident. Annie went to school, walked home, did homework, ate dinner, went to bed. It was unnerving, and I told Harden as much. I’d been seeing her more frequently as the end of the school year drew nearer. Harden, of course, couldn’t talk to me about her sessions with Annie, but she did indulge me on the topic. I went off about how Annie was a monster, and how the world would be better off without her in it. I was surprised when Harden stopped me and explained that I’d had my sister all wrong. How I’d vilified her for so long that I’d stopped seeing her as a person. This frustrated me.

“I’m not telling you that you’re wrong to feel the way you feel about her,” she reassured me. “What I am telling you is that you should try to understand who she really is. Right now, you see her as this…tornado. Just traveling along from town to town, destroying everything in her path for no reason. But I promise you, there is a reason for everything your sister does.”

“Like what?” I muttered.

“Well. Control, mainly. It’s what caused her to act out,” she emphasized with a wave of her hand. I could feel mine throb. “Annie needs to be in control of not just her own life, but everyone in it. And now, maybe for the first time ever, she’s losing a lot of that control. Anything can happen, and that scares her.”

I rejected this. “That’s true for all of us, and most people don’t do what she does.”

Harden gave a nod. “We’re all trying to figure out how to navigate through life. Your sister included. Not all of us were given the proper tools to do so.”

I thought about that for a moment. “Did something happen to her?” I pressed. Harden stared at me sadly, silently declining to answer. “Well what does she want then?”

Harden shrugged. “These are things you have to ask her. I think you two are long overdue for a conversation. You should really consider doing that soon. Especially if this surgery you mentioned does what it’s supposed to do,” she added with a hint of sarcasm.

I wasn’t sure I was ready for that conversation. If there was more to Annie, I had definitely never seen it. But I knew Harden was right. I was tired of being afraid of her. Of avoiding her in the halls, and at home. Tired of my entire life feeling like it revolved around her. I just wanted to live a normal life. With friends, girlfriends, birthdays, family parties, sleep. I felt like I couldn’t have any of that.

As we reached the last day of school, and the eve of Annie’s surgery, I’d reached the point where I could no longer put off the conversation I was supposed to have with her. I knocked on her door after an uncomfortably silent dinner.

“What?” she muttered.

There was a lump in my throat. “Can I come in?” I had to ask twice because it had barely come out the first time. She opened her door just enough for her body to squeeze through. “What?” she repeated.

“Can we talk?”

She paused, then moved out of the way, allowing me to enter. I’d only been in her room a handful of times since we were kids. It looked exactly the same now as it did back then. The walls were still pink, her old dolls still sitting high on the shelf, and her closet doorframe still had our childhood heights etched into the wood, something Papa used to do with us each time he’d visit. From here, Annie looked like a normal girl. I stood close to her door as she dropped herself onto the bed and looked up at me curiously. I was sweating. My hand, pulsating. I heaved a heavy sigh and decided the best way to do this was to just come right out with what I wanted to say.

“I want to understand you better.”

She didn’t blink. “I don’t think you do.”

“I do. I want to know what it’s like to be you. What goes on in your head. What you’re thinking. Why you do the things you do.”

“I don’t know,” she explained.

“What do you mean you don’t know?”

“Because I don’t understand myself either,” she said with more force. “You treat me like I’m an experiment, and I don’t appreciate it.”

“Annie, you’re about to get a fucking chip put into your brain,” I said shakily. She shook her head, and so did I. Talking to her could sometimes make you feel like you were the one who was crazy.

I continued. “You know you hurt people. I know you know that. Do you ever feel bad about it?”

“Of course I do,” she said.

It was clear I wasn’t going to get any truth out of her. “I don’t think you do. I think you hate people. I think you hate yourself. That you’re different. So you hurt people. Am I wrong? Do you even love me? Or Mom? Or do you hate us too?”

She looked at me like I was missing something obvious. She got up off the bed and approached me, stopping just a foot away.

“I don’t ‘anything’ you. I don’t ‘anything’ anyone.”

It was probably the most honest thing she’d ever said to me. In the moment, it made my skin crawl. It wasn’t until later that I realized how sad of an admission this was.

———

When Mom and Annie left for Boston early that Friday morning, I’d said nothing to her. Despite my doubts in Dr. McKinnon’s device, part of me was still hoping to receive a brand new Annie. With summer vacation now started and the house to myself for the weekend, I’d slept most of my time away, as though catching up on all the sleep lost throughout my life. I had no idea what to do with myself when I was awake. I’d watch TV, pace, eat, lie on the floor. By weekend’s end, I’d become so bored and anxious that I did something unexpected: I went into Annie’s room. Sat right on her bed where some clothes had been left strewn, nervous that she’d somehow figure out I’d been in there. I thought again about who exactly would be walking through the door when they got back the following morning. It kept me up that night. After a few short hours of sleep, I woke early, made coffee (that I don’t even drink), paced some more, and then waited in the same seat my Papa always sat in, staring at the front door as I mentally prepared myself for its opening. By that point, my mind had already been left to wander too far from reality. I’d imagined Annie bursting through to give me a hug and tell me through sobs that she was sorry for everything she’d done. It had occurred to me in that moment that we’d never actually hugged before, not that I could remember. When the daydream ended, I hated myself for letting her manipulate me when she wasn’t even around.

I heard car doors slam shut. My stomach sank. A few moments later, the front door opened and they entered as casually as if they’d run to the store.

“Oh hi, hun,” Mom beamed. “Didn’t expect to see you there.” She dropped her bags to give me a hug and kiss, and then added, “Annie, come say hi to your brother.” I wanted to puke. I could hardly bring myself to look at her. She was still standing by the door, looking bashful.

“Hi,” she mustered. She was rubbing up and down her arm, looking more uncomfortable than I was.

“Hi,” I replied. I finally looked her in the eyes. They looked different. A small patch of her head had been shaved, and I could see the end of the stitches running down her scalp to the edge of her forehead.

Mom sighed at our silence and started rummaging through kitchen cabinets. “Well, I know it’s lunch time, but I’m making breakfast. Anyone hungry?”

“Can I go take a shower, Mom?” Annie wondered.

“Of course, baby. Just be careful, you can’t wet your head yet, okay?” Annie nodded and quietly disappeared upstairs. Mom waited until she was long gone and then hovered beside me as bacon sizzled on the stove. “They said it could take a while to kick in,” she whispered excitedly. “But I think it’s already working!”

I remained silent as she continued with the eggs and bacon. “Where’s that knife?” she suddenly exclaimed, staring at the wooden block on the counter. The biggest slot was still empty. I wasn’t planning on putting it back just yet; despite my mother’s optimism, I was going to need to see a lot more.

I wouldn’t see much in the weeks following. Annie spent most of the time asleep, an expected side-effect. She was pleasant but quiet at dinner, uttering ‘please’ and ‘thank you’ but not much else. I’d been trying to enjoy summer break as much as I could, shooting pucks out in the driveway, riding my bike around neighboring towns, and even saw a movie with my friend from school. My deal with Mom was that I’d stay home during the day while she was at work, in case Annie needed anything. I wasn’t thrilled about being left alone with her, but I hardly saw much of her at first. Just a couple quick greetings in the hallway, nothing more. Mom was frequently calling to check in but there hadn’t been any issues. Until I shot awake to the booming sound of things crashing against the walls. I ran out into the hall and stood outside Annie’s door, listening as more things got slammed on the other side. She was throwing an absolute tantrum. I was about to enter but thought better of it. Then, as soon as it had begun, it was over. Silence. When I called Mom to tell her what happened, she told me that these kind of outbursts were expected. ‘Emotional fallout’, Dr. McKinnon had told her. I wish someone had told me.

Going forward, I was hyper vigilant. Thought I’d heard Annie through the walls one day, talking to herself. I pressed my ear against it but struggled to make anything out. This would happen again and again, day after day, this very faint whisper among the sound of gasps and coughs. And each day it got louder. So I stood outside her door again, lost in the white noise of the fans and air conditioners buzzing in the distance, Annie’s mumbling creeping from under her door. I wanted nothing to do with her, and yet I was curious. So I knocked. There was a pause.

“Come in,” her little voice called. She was wrapped in her sheets, in the dead summer heat, with only her face poking out. “Hi,” she whispered as I stepped in. I stood right by the door, just as I had the last time she let me in.

“Are you okay?” I asked half-heartedly.

Her face immediately scrunched up in a way I’d never seen it. “No,” she squealed as she shook her head and started to cry. I tried not to show how good it made me feel, to see her suffer. She got louder, so I approached the bed.

“What’s wrong?” I asked as I stood awkwardly over her.

“I don’t like this!” she choked through sobs and sniffles. “I don’t like it… I don’t like it…”

She reached for my hand and kept repeating herself. I was stunned. “It’s okay,” I said, but didn’t really mean. As I sat there holding her hand for a while, uttering fake assurances, not really caring, I wondered if the way I felt in that moment was the way she’d always felt. If so, I didn’t envy her.

Later that night, it was Annie who knocked on my door. She slipped in like a cat, crawling up onto my bed and sitting there with her legs crossed. It was fairly muggy but she was still in a hoodie and sweatpants.

“Sorry about earlier,” she said.

“It’s fine.”

“It’s not fine. I know you hate me. You don’t have to act like you don’t. I just wanted to tell you that you were right. I hate myself too. And I was jealous of everyone. You asked what it was like to be me,” she began. My ears perked. “It’s like…being a ghost. Floating around. Lost. You don’t remember who you are or what it was like to be alive. You just exist. And nobody even knows you’re there. And when they do see you, they get scared. They don’t want you around. So you stay in the background and watch everyone live their lives. It’s not fair. So you mess with them. For attention. Because you’re bored. Beyond bored. Because for just one second, their screams make you feel like you’re real. You chase that feeling.”

I was blown away, unsure how to respond. I just sat up against my headboard in awe. The knife under my pillow was showing for a second before I shuffled to cover it. “Wow. I wish you could’ve told me that a long time ago. But I don’t hate you, Annie. I’m afraid of you.”

This hit her in the gut. She wrinkled her face and I worried she was going to cry again. Instead, she took a deep breath and smiled, like a switch had been flipped. “Can I throw you a birthday party?” she suddenly blurted.

I was confused. “My birthday’s in two months.”

“I know but…can I do it anyway? I want to do something nice for you. Please?”

I had no idea what to think of this, or of her. But she was staring at me wide-eyed and hopeful. “Okay,” I said, annoyed. She clapped her hands and thanked me with a giant grin on her face.

Later that afternoon, Mom took Annie shopping for decorations and a cake, which felt ridiculous to me. When they returned, they kicked me out of the house for a while so they decorate. I took a long walk around the neighborhood, even stopped at a park to watch a little league baseball game. I’d never played before and was kind of wishing I had. When I got home, I was amazed at what the girls had done. The entire kitchen and living room were lit in a multicolored glow, with lava lamps, strobe lights, and glow sticks all around the room. There was a “Happy Birthday” sign hanging on the center wall, and on the table below was my cake, chocolate with vanilla frosting, already lit with a number sixteen candle. They started singing, and then laughing at how stupid this all was. Annie couldn’t stop. She laughed so hard it almost made her look crazy. Though I wanted no part of this, I put on a face, for my mother. For the first time in our lives, we were going to have a good night together, and I wanted to give her that. We had some awkward chit chat, and even more awkward reminiscing, as Mom told stories of past birthday parties. She’d left out the parts where Annie had found ways to ruin them.

After having cake, Annie ran up to her room real quick and came back down with a small present, wrapped and topped with a bow, handing it to me without a word. It surprised me, but not nearly as much as what was inside. In the little box was a very familiar pin. Papa’s medal. All those years I thought I had lost it, and she fucking took it. I was overcome with a range of emotion and wasn’t sure which was going to come out. The look on my mother’s face said it all, as she was silently begging me not to react negatively. Annie was waiting tentatively. Part of me was ready to yell at her, but when I took the pin out and held it in my hand, the rage went away. I was just so happy to have it. I gave her my best thanks, and she lunged forward, wrapping her arms around me in this long, quiet embrace. Mom watched on with her hands covering the wave of emotion that had hit her. When we settled, we ate more cake and finished the night playing some inappropriate game Annie had convinced Mom to buy. I couldn’t take my eye off my sister. I wanted to catch her in an unsuspecting moment, just to see if the mask would show itself. When her attitude suddenly shifted to a somber state, I couldn’t tell if it was due to my watchful eye or if it was just another instance of emotional fallout.

I’d heard Annie again that night, quietly crying herself to sleep. In fact, I’d been hearing it almost every night. It was becoming less enjoyable. I thought about how if any of this was real then it meant she’d been in a lot of pain for quite some time now. When I realized I was starting to feel bad, I caught myself. I couldn’t let her fool me. And she wasn’t going to give up trying. She’d asked me what else she could do to fix our relationship, and I admitted to her that, even if her surgery had worked, it was hard for me to separate who she was now from who she was before. She understood. The very next day, she dyed blonde streaks into her hair.

As the summer wound down, I hung out with her a little more. Movies on the couch, midnight conversations in our rooms. I tried to limit it. But she was like a puppy, following me around for attention. For all the questions I used to have for her, she’d had that many more for me. Simple things, like my favorite food, or who I’d had a crush on. She even joked about how she’d probably once known this information but didn’t care enough to remember it. I was starting to get tired of playing along. So I put her on the spot and asked about the nightly crying. She seemed hesitant at first but then explained that she can never fall asleep anymore because images of all the pain she’s caused keep her up at night. She said every time she thought she’d remembered everything, something new would pop up. I nearly rolled my eyes. But that small sliver of hope in the back of my mind made me tell her that if it were ever truly bad enough, she could just knock on the wall three times and I’d come to her room and sit with her. She thanked me with another long hug, and I’d hoped to not deal with it any time soon.

She knocked that very night.

On the final week of the summer, my one friend invited me to go to his family’s lake house. Mom wasn’t sure she wanted to leave Annie home alone yet, but both Annie and I assured her she was fine by now. I guilted Mom over how I’d hardly done anything that summer, and that worked. I was gone for five days of jet skis, hot dogs, and fireworks. I’d told my friend everything that had happened that summer, probably more than I should have. “I should’ve invited her too,” he’d joked. I told him if he had, he’d probably have “accidentally drowned” by now.

When the week ended, they dropped me back home. It was mid-day and Mom would’ve already been at work. I couldn’t imagine how often she’d checked in on Annie. But when I got inside, she was nowhere to be found. I called out, but nothing. I checked upstairs, even opened her door to see if she was asleep. Still nothing. Then I heard this strange buzzing sound coming from downstairs. I followed it to the basement door. It was locked. I banged on it and called Annie’s name. The buzzing continued. Then I heard this painful, horrific scream. I started punching the door repeatedly, shouting. I didn’t know what to do. I kicked the doorknob, over and over until the door cracked at the hinge. When I got it open, I skipped down the stairs and rounded the corner to see Annie with her head on dad’s workbench. She was holding one of the power drills, with the drill inside her head where the scar had been unstitched, right above where the chip had been placed inside her skull. Blood was spattered everywhere.

“I want to go back!” she shrieked. “I want to go back!”

———

Annie was rushed to the hospital, where she stayed for a while. She hadn’t punctured too far, but they wanted to keep an eye on her. When she was released, Mom brought her right back to Dr. McKinnon, who was in awe over what his patient had done. He almost seemed proud as he tried to spin the incident as good news, that at least the device was clearly working. Mom wasn’t so thrilled. She was hoping for a way to lessen its affects on her poor daughter, to which he could only offer medication. Much like her previous doctor had said, McKinnon explained that Annie needed more time. That she wasn’t just learning how to live with those around her, but with herself as well. He reminded us that she was feeling her entire life’s worth of guilt and shame, and said that the best thing we could do for her now was to help her heal. And maybe keep a closer watch in the meantime.

When we got home, Mom found Annie another therapist and transferred her to a new school. Annie was going to go to St. John’s Prep after all. Mom had to dip even further into whatever we’d had saved, but she wanted to keep Annie as happy as possible and figured a fresh start was in order. This, in addition to the medication, calmed Annie down a bit as we got ready for the new school year. I hung out in her room with her through the final days of summer break, just to keep watch. I was told not to talk about the incident, but Annie was the one who brought it up. She’d suddenly asked me how I live with my remorse. I didn’t know how to answer that, it seemed like something for her new therapist. But I told her the best thing she could do was to learn from it. To just be better today than she was yesterday. It was corny and not nearly enough, but she thanked me. Then she asked if I loved her.

“Not yet,” I said honestly. “But I’d like to someday.” And I meant it.

She hugged me anyway and said, “I’d like that too.” She was happy enough to leave it at that.

On the morning of the first day of school, Mom and Annie were up and moving pretty early, which meant I, too, was awake. St. John’s started earlier than my high school, so they were ready to head out the door before I’d even had breakfast. Mom grabbed her keys off the table and kissed me as I crunched cereal. Annie was standing by the door in her new uniform.

“Don’t forget to lock the door, okay?” Mom said to me. “Have a good first day. Hey—the knife showed up!” She paused at the sight of it. I’d finally put it back into the block that morning.

“It was in the drawer,” I lied. Mom laughed it off and said bye. I looked up to wish Annie good luck but she’d already had her eye on me. I worried that she could tell I was lying, or that she’d seen the knife in my room that day. But she was smiling. She said bye, and the two of them walked out. In that moment, I was actually really happy for my sister, and for her new friends who’d have no idea who she used to be. None of that mattered anymore. Annie was a normal girl about to live a normal life. And I was going to live mine.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "0760686a-6598-43e6-9ed7-87f340c78d7c",
                    ThreadTypeId = 1,
                    Title = "I pretended to be a missing girl.",
                    Content = @"
Mikayla Murray went missing twelve years ago, on the eve of her 18th birthday. She didn’t have any big plans or anything, but her friends described her as having been in a particularly good mood for what was an otherwise perfectly normal Friday. She’d gone to school, soccer practice, work, and then came home for a night of movies with her kid brother, James. He was more excited for her birthday than she was. Even wanted to stay awake with her until midnight but, of course, had fallen asleep right away. When he woke in the middle of the night, he saw her headlights shining through his window and watched as they rushed down their country road, not knowing that it was the last he’d ever see of her. The poor kid was only five and would be forever tormented over why she’d left him, or why she’d never come back.

It wasn’t until the sun came up on that cold Saturday morning that anyone realized something was wrong. Her parents entered her room to wish her a happy birthday, only to find her bed empty, car gone, and phone off. They’d started their rounds of calls to all Mikayla’s friends, but nobody had seen or heard from her. Panic really started to set in when Mikayla’s car was found abandoned on the side of a heavily wooded road, facing the wrong direction, practically in the middle of nowhere. There were no parks or hiking trails, nor were there any signs of a struggle, or any evidence of where she might have gone next.

Until Mikayla’s parents followed that road on a map. They knew she had a boyfriend, Tom. He was a year older and had just gone off to college. He’d been trying to get Mikayla to come visit him but her parents forbid it. But if they hadn’t, this was the very road Mikayla would have taken to get there. So while Linda Murray filed the missing person’s report, Paul Murray sped on up that road, all the way to Tom’s university. Tom swore to him (and, later, the investigators) that he hadn’t seen her in weeks. That he’d been in his room studying that night. His roommate confirmed as much, with the added disclosure of having later gone home, where he’d then spent the weekend. The rest was uncertain. The police looked deeper into Tom and found strands of Mikayla’s hair in his car, which proved nothing foul, but it spooked him enough into admitting that he’d seen Mikayla more recently than he’d stated. That he’d picked her up late the weekend prior for a midnight drive. This sounded precisely like what had happened the night she’d gone missing, but police found nothing to substantiate it. Tom was eventually cleared as a suspect, and the Murrays would never let it go. They were certain he was involved in Mikayla’s disappearance. So certain, that Paul Murray spent several nights sitting outside Tom’s dorm, waiting to catch his daughter going in or out. Tom’s family wanted to press charges, but Paul had friends in the Sheriff’s Office, who’d assured the family that it would not happen again, and left Paul with a very stern warning. But being friends with law enforcement only went so far, as the case would soon go cold, with days, weeks, and months passing by without any further updates. The public moved on, while the people in Mikayla’s life were left with this dark cloud of uncertainty, wondering what had happened to her. If she was out there somewhere, alive.

And she was. She was about to return home after more than a decade gone. Because I’m Mikayla Murray, and I ran away that night to start a new life.

That’s what I told the Murrays, anyway. I had no fucking idea what happened to that girl.

———

I’m awful, I know. I’m not proud of myself. I was desperate. Homeless, and on the run. Smoking a pack a day. Sleeping with men from bars for money, only to spend it at another bar and do it all over again. I was stuck and needed a plan. Then I saw her face. Mikayla Murray. It was on a bulletin board at some cheap motel I’d been passing through. There were half a dozen girls on there, but Mikayla stood out, her blonde hair straight and pretty, her blue eyes as wide as her smile. It stopped me dead in my tracks, because she looked like me. Exactly like me. I could’ve swapped in one of my old high school photos and nobody would’ve noticed. Not that anyone was paying attention to this board or these girls anymore. Even the lady at the motel, who’d spotted me staring, said, “They ain’t comin’ home, dear, but I don’t got the heart to take’em down.” I was curious, enough to turn on the phone I’d kept in my bag just in case. My father had long stopped paying for it, but the motel offered free WiFi and I’d used it to read more about Mikayla. I learned that she was only two years older than me, and that the photo in the lobby wasn’t just a one-off. She resembled me in every other photo, of which there were many, along with theories about what had happened to her. I couldn’t have given any less of a shit about that rabbit hole. What got my interest were the earrings Mikayla wore in these photos, or the necklace her mother wore at the press conference, or the watch on her dad’s wrist. As I dug deeper, it became clear that the Murrays had money, a fair good amount of it. After entertaining Jerry from the bar (and stealing his jacket), I ripped a butt late that night and decided… one of those girls was coming home. And it was going to be me.

The Murrays still lived in the same house, an hour west of the small Michigan towns I’d been nesting in, which worked perfectly, as I’d been toying with the idea of going back home to Chicago. It was a cheap way to justify the awful thing I was about to do, because in reality there was no fucking way I was actually going back home, even with Murray fortune in my pockets. It’s frightening what we’ll do to ensure we’re the good guys in our story. As I dished out a small chunk of my remaining cash to hop on a bus, I felt no hesitation, or fear. Sure it was risky, but I wasn’t planning on being there for more than a night. I’d done enough research on Mikayla to get in, find what I could take, and get out. I was going to beg the family to give me one day before alerting anyone that I’d returned. To let me rest in my own bed before being swarmed by whatever media Nowhere, Indiana had to offer. After miles and miles of cornfields, I’d hoped to have plenty of time to escape that wave. When the bus arrived at the station, I could’t help but notice how out of place it looked, like it had been copied and pasted from somewhere else, standing out among the run down outlets, shops, and restaurants. I spotted a seedy looking bar next to an even more questionable looking mechanic and thought about making a detour. I needed a drink. But I couldn’t. I had to make sure not to talk to anyone. I couldn’t risk being mistaken for the town’s longest missing girl, not here, not now.

So when an older man approached me outside the station as I smoked one last cigarette, I’d panicked. He asked if I could bum him one, said that he really needed it. So I did, just to make him go away. Then he started rambling on about his car having broken down in this shithole and how he was stuck here until they fixed it. He told me his name and then asked me my mine. I told him it was Abby. It’s not. He said I’d reminded him of his niece back in Iowa, something I pretended was interesting. Maybe I can pretend to be her too, I thought. When I finished smoking, I wished him luck and set off for what I came here to do. I shoved the rest of my cigarettes and lighter deep into my backpack, along with my real identity, and when I turned down Lincoln Ave, I was no longer me. Or Abby. I was Mikayla Murray.

———

The Murray’s lived a pretty secluded life. Their home sat alone in the middle of endless planes, their neighbors barely dots in the distance. I was starting to understand why Mikayla might have run away. Although, the house itself was beautiful, with many protruding sections and gables, a wraparound porch, and a large, two-door garage. There was even an inground swimming pool out back (now covered and topped with autumn leaves), and a cute little gazebo further off in the field, draped in numerous flags and dreamcatchers, with flower pots lining the railings. It certainly didn’t look like the kind of place tragedy had struck.

I stepped quietly up the stairs and was almost spooked by my own reflection in the glass of their front door. Nerves were definitely setting in now. I rang the bell and felt my stomach sink. What if my dirty-blonde hair wasn’t light enough, or if Mikayla had had some obvious birthmark I’d overlooked? I was sweating underneath my coat, unable to recall the name of the man I’d taken it from. When the door opened, my heart stopped. Linda Murray was standing there in her casual weekend wear, pleasantly confused.

“Hello,” she greeted me. Then her face went white in an instant, like her soul had left her body. She shrieked and clasped her hands to her mouth, bursting into tearful exclamations. “Oh my god!” she kept repeating. She suddenly lunged forward and squeezed me tighter than I would have liked, her arms attempting to wrap all the way around my backpack. I stood there awkwardly, bracing all of her weight onto my mine, as she surely was about to collapse. The dog at her legs was barking madly, and, as Linda’s tears dropped onto my back, all I could think about was how pissed off I’d be had I gotten caught because the fucking dog didn’t recognize my scent.

“What is it, Linda?” Mikayla’s dad called from somewhere inside. He soon appeared in the doorway, his button-up tucked into his jeans, and when saw my cold, pale face poking over Linda’s shoulder, he stumbled back.

“What is this?” he gasped. His eyes went wide and his bushy grey mustache twitched. The dog was still barking, reminding me that I was in fact a stranger in this house.

I smiled and said, “I’m home, Daddy.” I was trying to make myself cry, and if Linda had squeezed me any harder, I just might have. She held onto my sleeve as we let go, as though afraid her daughter would run off again. Paul Murray was still staring at me in disbelief, when something shifted in his face and he stepped forward.

“Come here, baby girl,” he uttered. Linda passed me off like a toy she did not want to share. Paul pulled me into his arms and held even tighter than she had. We rocked back and forth for a moment. “I can’t believe it’s you,” he whispered. Linda rushed for the door and yelled inside, calling for her son James.

“Come inside, baby,” Paul beamed as he released me, keeping a hand on my back and beckoning me inward. “It’s cold, come!”

We moved into the foyer where Paul asked to take my coat, which I happily handed him. Now that I was inside, I could practically smell the bar on it. “Your bag, sweetie?” he added.

I shook my head and said, “No, that’s okay.” He made a face and I worried it was suspicion, and then worried more that my worrying was the only thing suspicious. I had to settle down. I’d nearly jumped when Paul turned the locks and hit a button on the alarm system. It chimed louder than I would have expected. I wondered if all this had always existed or if it was a result of their daughter having slipped out one night, never to be seen again. One of her coats still hung on a hook by the door, untouched after all these years. Now mine hung next to it. Well, Danny’s or whatever.

As we moved even further inside, I was blown away by how nice this place was, so much so that I’d slipped and let it show; nothing in this house was supposed to be surprising to me. It was hard not to be impressed by the high ceilings and book shelves, or the many sofas beside a grand marble fireplace, or the fact that this was just the room that branched off to all the other rooms, one they’d probably hardly ever used. As I continued to survey my surroundings, a figure high above caught my eye. It was James. He looked down over the railing and looked more flabbergasted than anyone to have seen me. At seventeen, he was now the same age his sister was when she vanished, only much taller, but with the same baby face.

“Look, James! Look who it is!” Linda cried joyfully. “It’s sissy! Come give her a hug!”

I wanted to puke. James didn’t move right away, and when he did it was this slow, cautious crawl. I figured surely, of all people, I’d have been safest around James. After all, he’d hardly ever known his sister. Yet the baby blue eyes behind his jet black hair were piercing into mine, searching for the girl he so dearly missed. I couldn’t think of what to say to him, and was distracted by the feel of the cigarettes in my bag. I needed one.

“Hi,” was all he mustered, stopping at the foot of the stairs.

“Hey goober,” I replied. I had no idea if that was something Mikayla ever called him, but neither had anyone else. James and I then did something resembling a hug and let go. Linda looked on, face red, still overcome with emotion. Paul was smiling at us.

“Let’s go sit, yeah?” he suggested. “You look exhausted.”

He wasn’t wrong. I couldn’t wait to sit down. There probably wasn’t a piece of furniture in this place less comfortable than the mattresses I’d been living on for the last decade. I held in my amazement as we marched from room to room, deeper and deeper into the house. Linda was still exhaling this stuttered, painful sob, and kept reaching to touch me in any way, a hand on the back or a light brush of the hair. It was annoying, but then again I’d never learned how to have a mother. When I shrugged Linda off, she looked heartbroken. It was at that moment that I finally began to feel like the asshole I knew I was.

After passing through the kitchen and down another hall, we stopped in their second, larger living room. It was very open, the ceiling reaching all the way up to the third story, with photos lined as high as a ladder could reach. I followed Mikayla’s progression of school photos, remarking how eerily similar they were to mine, and how they were one photo short. There was an upper level beside us, where a grand piano sat in one corner, and a bar in the other, separated by yet another fireplace. I imagined how nice a Christmas tree must’ve looked in here, even during the day with the natural light coming in through the sliding glass doors to the back porch.

Each Murray dropped onto a separate couch on the lower level. Paul gestured for me to sit next to Linda, who, of course, was eager to be next to me. James was slouched directly across, staring down at the ground. The rest of us were darting our eyes, waiting for someone to begin.

Paul cleared his throat. “Let me just start by saying that… we’re not mad.” Linda was nodding feverishly in agreement. Paul went on. “We just want to know what happened.”

Something inside my gut wrung. If my actual dad had showed even an ounce of this concern, I might not have run away myself. Instead, he took his brother’s side. My abuser.

I dropped my head. “I needed to get out of here. I felt trapped. I didn’t mean to hurt anyone, I just didn’t know what else to do. I’m really sorry...” It certainly wasn’t Oscar-worthy, but I wasn’t playing the long-con. I only needed to be passable long enough for me to swipe several of the items we’d passed along the journey to this room.

Paul nodded slowly, gazing off somewhere over my shoulder. “Okay,” was all he said. It was somehow worse than anything else he could have said. For all I knew, Mikayla had had a great life here, with a loving family. Now I was making them feel responsible. Each of them was staring off somewhere, letting my story sink into their minds. I wanted to sink into the couch.

“Where did you go, Mikayla?” Linda suddenly wondered.

Paul leaned forward. “No, Linda. It’s okay. She’s not a little girl anymore. That’s her business. Listen baby girl, we don’t have to talk about it if you don’t want. All that matters now is that you’re home, and you’re safe.”

He reached for my hands and held on gently. It was strangely comforting. For the first time in my life, I’d felt cared for, and safe. In my short time there, I’d completely flipped my thinking. What if Mikayla was just another stupid teenager rebelling against parents who were only trying to protect her? What if she’d sneaked out to celebrate her 18th birthday with her college boyfriend at some frat party? What if he slipped something into her drink? Or if she got too experimental? What if someone offered her something she’d never tried before, and she took it? To be cool? To show off in front of her college boyfriend’s college friends?

I’d spent my whole life wishing I had hers. What if she’d just left it?

“You know what? I have an idea,” Paul said with a clap. “Linda, why don’t you go out and get stuff for pork sandwiches? I’ll cook up some tater tots? Yeah?” He was looking at me with raised eyebrows like I was supposed to know what the fuck he was talking about. So I pretended to. This must have been some sort of Murray tradition or Mikayla’s favorite meal.

“That sounds great,” I replied. I tried to smile at James but it was clear he wasn’t ready to forgive his sister for abandoning him.

Linda hopped up. “Mikayla, sweetie, do you wanna come with me?” I hated how often she was saying her name, and how she spoke to me like I was five.

Before I got a chance to respond, Paul chimed in. “Hun, let her breathe. Run to the store, I’ll get things started here, and you,” he said to me, “go rest up. It’s gonna be crazy here by tomorrow. I just wanna have one night as a family first.”

I could not have agreed more. Everything was going exactly as I had planned, maybe better. There was a really shiny, diamond-studded vase across the room calling my name, right next to an autographed jersey of some football player I’d never heard of. I was gonna walk out with one while wearing the other.

“Go on up to your room,” Paul said to me. “We’ll come get you when it’s ready.”

Linda pulled me in for another hug and kissed me on the side of the head. She looked over at James and saw that he was looking rather lifeless. She caught his attention and made a tipping motion toward her mouth, to which James replied, “I took them already.” He finally glanced my way, but it wasn’t quite the look I wanted to see. There was more than just betrayal in his eyes.

Everyone broke at once and dispersed, Paul heading for the kitchen, and Linda making her way out. I grabbed my backpack and followed a sluggish James up the stairs, feeling good about how things were going so far. Until it occurred to me that I’d had no idea which room was Mikayla’s. It wasn’t something she’d ever have forgotten, not even after twelve years. James and I rounded the corner and were faced with a long, narrow hallway with several doors. I feared I was going to have to guess the right one, when James threw me a lifeline.

“Hey,” he began. He’d stopped in front of his door and turned to me. “Do you want to hangout? Watch a movie or something?” Even this had come out tense, like he was being forced to ask. Then I remembered that this had been the last thing he and Mikayla had done together.

“Yeah, sure,” I said happily. “I don’t think I’m ready to see my room yet anyway.”

James nodded, and the knot in my stomach untwisted. When we entered his room and I was surprised by how neat it was, so much so that it felt wrong laying my dirty bag down. James’ baggy jeans and messy hair gave me a totally different vibe, but his bed was made, the walls were bare, and the desk in the corner looked like it had hardly ever been used. The one window in the room had a perfect view of the setting sun beyond the fields. Its shadow cast a line between the pool below and the gazebo that was just barely visible from this vantage point.

I heard a lock click.

“We need to go, now,” James whispered. He let his neutral expression drop into one of panic. I watched in confusion as he rushed over to his closet and threw on a sweater, cursing under his breath as he did so. When he looked up at me again, it was like he’d forgotten I was there.

“What are you talking about?” I demanded.

James shook his head. “He knows. He knew the whole time.”

My brain was automatically rattling off ways to salvage this, but there was no point. I was caught, and something other than my identity was bothering him. That made me nervous.

“What gave it away?” I wondered.

He looked at me like I was crazy. “Do you have any idea what’s going on here? He killed her! He buried her!”

My heart stopped. “What?” That wasn’t in any of the theories I’d read online. Like everyone else, I had been so sure it was the boyfriend, Tom. It was obvious. But the look of fear washing over James’ face was hitting me as well. “How do you know that?”

He took me by the arm and dragged me to the window. “Look,” he spat with a outward finger against the glass. Far beyond the covered pool sat the gazebo, lifeless and weather worn, with noticeable chips in its white paint. Only its right side was visible from behind the rest of the house. I could see the flags encircling its beams, waving calmly above a row of gardening supplies. James was breathing heavily as he stared out at it, his eyes fixed, even as he spoke. “He built it right after she disappeared. And we’re not allowed to use it. Calls it his garden. I climbed it once when I was ten and he beat the shit out of me. But I’ve seen him out there at night, a couple times. Spraying the plants. Fixing the dirt. And look—” He hurried to his dresser and rummaged through before pulling something out and jamming it into my gut. I reached down. It was a dirty, purple bracelet, all stretchy and rubber. It had Mikayla’s name on it. I played with it in my hand.

“Remi dug that up last year,” said James. “Dropped it right at my feet. And I remember it! I remember her wearing it that night!”

I stared at it and let it slide down onto my wrist, trying to find any counter to his theory. “You saw her drive off,” I reminded him.

“I saw her car drive off.”

There was a voice in the back of my mind telling me he was delusional. But the voice that believed him was louder, and much more afraid. I watched, mouth agape, as he struggled to tie his shoes. He kept messing up and starting over, spitting more curses under his breath. My thoughts were swirling. “James, why haven’t you called the police?”

“Because I can’t!” It was louder than he’d intended. He stood up and recollected himself. “My dad is friends with the sheriff. If a cop pulls up, he’ll kill us. If I run, he‘ll—he’ll—kill my mom! I don’t even think she’d believe me!”

I put my hands up to quiet him but the fearful cry he’d been holding in had burst out. He covered his mouth to push it back in, along with the snot and tears that were oozing out of him. He rushed over to his bedside drawer and picked up a bottle of pills, swiftly popping a few into his mouth. My chest was getting tighter. “Why can’t we just play along a little longer?” I said. “I’ll leave tonight.”

“If we go downstairs, we’re dead. We were dead the second you got here. The police, the news, they’ll all be here tomorrow whether you’re here or not. They could find her. I could tell them. My dad might snap, like he did on Mikayla. You don’t understand, he’d rather die than get caught. And he’d take us with him, I know it. So we need to go. Out the window. Now. We’ll just run.”

At that point, it no longer mattered to me if James was right or if he was out of his fucking mind. I wanted to get out of there. I looked out the window again. It was starting to get dark. I would’ve preferred a more casual escape over jumping off the roof, but if what he was saying was true then we had no choice. I was craving a cigarette more than ever, and that alone was almost enough to get me on that roof.

“We could take a bus,” I said. “It’s how I got here. It’s only a couple miles that way.”

“I know where it is. We just have to stay off the roads. We can use the fields for cover, and then once we—”

There was a knock on the door.

“James?” It was Paul. He tried turning the handle. “Everything all right in there?”

Without hesitation, James ran for the window and opened it. I could hear the faint chime of the alarm from somewhere out in the hall. Paul’s jiggling of the locked door grew more aggressive. “What the hell is going on in there? Open the door.” When he started pounding on it, I threw on my bag and joined James, who was already halfway out the window. Together we scurried on the roof, hopped onto the back porch, and dropped down onto the rather large portico above the back door. I’d almost fallen down the side but James held me up. A loud crash came booming from back up in his room. I looked up.

“Don’t stop!” James yelled.

He jumped first onto the lawn, and I followed. Both of my feet and knees took the impact hard, the ground underneath the autumn foliage deceptively solid. James helped me up, and we took off running. Against his advice, I looked back and saw Paul peeking out James’ window. He shouted to us and then disappeared. James had already separated himself a good distance from me. I kept pushing my legs as he called back for me to do so, my backpack bouncing off my ass with each stride. There was a road in the distance, the same road I’d trudged along to get here. I could see where it met the orange and purple sky. It felt like it was never getting any closer.

A gun shot rang loud, ripping across the plains. Paul was now standing by the back door aiming a rifle in our direction, his cries chasing us behind the gunpowder. Another bang and my legs buckled. When I’d reached the gazebo, I hid behind it to catch my breath. There was a sharp, debilitating pain in my side. I held myself up on one of the railings and thought I could feel my heartbeat vibrating against the wood. I swore if I’d survived this, I would quit smoking. I peaked around the corner and saw Paul hurry into the garage, James calling out for me by the road. But as I stood there frozen against the gazebo, flags caressing my shoulder, I thought about the girl buried underneath. What if James was right? What if nobody ever found Mikayla’s remains? What if we didn’t make it out of there, and nobody ever knew? I thought of my own story, unheard and not believed. When I’d told my dad what my uncle had done all those years, he hit me. I couldn’t let this story stay buried too.

I took out my lighter and lit a flag, and then another. The fire burned slowly, picking up quick as it caught onto more flags and dreamcatchers, then down to the plants below. I stumbled back and watched the flames spread and dance along the darkening sky. I hoped, at the very least, it would be a distraction, and more so enough to attract law enforcement. I could hear Linda’s shrill voice crying out at the sight of it. She’d rushed around the side of the house with groceries in her hands, calling out for her husband who had just sped off in his truck. He was coming.

I made one last dash for James, who was impatiently waving me on. He took my arm and led me across the road his father would soon be turning onto. We slipped into the cornfield and kept going until we heard the roar of an engine pass by. We froze until there was nothing but the wind, the pain in my side still nagging me. As dark as the sky had rapidly grown, it was even darker in that field, the corn towering over us, clinging to life as much as we were. Paul’s headlights were shining through from not much farther ahead. We waited in terror, for a crunch, or a shout, or, ideally, for the truck to zoom off. Another gunshot rang high into the air. I gasped and had to cover my mouth to quiet my breathing.

“What are you doing with my son?” Paul called in a singsongy fashion. We could hear him walking about over the sound of his engine purring. “Where the fuck are you?” His footsteps wandered around, farther, closer, then farther again, separated only by the sound of swishing corn as he searched randomly along the outer edge. There was a pause, followed by a door slamming shut. Paul’s truck whirled and sped back down the road. I exhaled as James tugged on my arm and instructed to keep going. We pushed through more corn and followed along the road as best as we could. I never would have imagined being in this situation when I’d walked down it earlier that day. Now I was wishing I never had.

Sirens suddenly wailed nearby, and eventually rushed past us. The glow of the flames had grown noticeably brighter in the distance, the smoke visible high above the fields.

“Holy shit,” James gasped. “C’mon, we need to keep moving. Are you okay?”

“Yeah...” I wasn’t.

We shuffled farther through the corn, shoving it aside more aggressively as we went. I could hardly see more than a few feet in front of me. After a while, we could hear a steady buzz of passing vehicles, indicating that we’d reached the city, but also the end of the cornfields. We stepped out onto a road and into the glow of street lights. I felt like I could breathe again, for just a moment.

“You ready?” he said. “We gotta move quick, but we gotta blend in.”

I’d realized in that moment how truly young James was, and how insane I was for having put my life in his hands. I was twenty-eight, but felt just like the same little girl I was all those years ago, hoping her father would protect her. I’d only hoped James was better at it.

We dashed across an empty street and then slipped into the downtown area. I kept my head down. Most of the businesses on the strip were closed for the night, but the bar I’d seen earlier was now glowing in its neon signs, which did a good job masking its otherwise unapproachable façade. There were locals standing outside having a smoke, drunkenly arguing about nothing. James and I crossed the street, and when we reached the bus station, I was relieved to see the lights were still on. This relief would not last.

“Incoming only, folks,” the man at the desk told us. “You’ll have to wait until morning. Sorry.”

I was already making my way for the exit. James caught up with me. “What are you doing?”

“I’m getting the fuck out of here.”

“What am I suppose to do?” He followed alongside me, being more conspicuous than I would’ve liked.

I stopped and leaned in close. “I don’t fucking care. Come with me, or don’t. I’m leaving!”

I was on the verge of crying, the lump in my throat growing larger. James stood there at a complete loss. I looked at him and saw the five year old boy who woke up that night all alone, the streetlights above shining in his eyes like the headlights he’d watched disappear. “I’m sorry,” I added. “I just wanna go home.” I couldn’t believe I had said it, and meant it. Then I realized James couldn’t go home. If my fire failed, he was going to be on his own, on the run, and homeless at seventeen. Just like I was.

“They’re going to find Mikayla,” I assured him. “Then you’ll be safe.”

He was trembling. “What if they don’t?”

I had no answer, not one he would have liked anyway. Even if we’d made a call to the police that very moment, I could already see Paul going home and putting the rifle to Linda’s head before putting it in his mouth. I’d wondered if he already had. I think James did too. He leaned into me and started to cry. He was a whole foot taller than me, and boney, but I held onto him, not like I had with Paul or Linda, but with earnest.

“Excuse me!”

A voice suddenly called out to us. I was about to run when I saw a familiar face. The old man I’d smoked with earlier was approaching us from the mechanic’s lot next to us. His face twisted when he recognized me back. “Oh, it’s you! Abby, right? Everything all right over here?”

James and I looked at each other but said nothing. An idea crossed my mind. “Actually no, sir. We’re stuck here too.”

The man, whose name I’d forgotten, grinned. “Huh. Well. Car’s fixed! I’m about to head out if you guys need a lift. Where ya’ headed?”

“Anywhere,” I begged.

His smile faded. “Right. Okay. Sure. That’s fine. I’m gonna be driving west down 80 for a while, if that works for y’all.”

“Yes,” I said. “That’s perfect.”

James and I followed the man back to the lot and hopped into his old station wagon. I took the front. I thanked the old man repeatedly, even offered him gas money, but he refused it. Said he was happy to help. He introduced himself to a catatonic James in back, reminding me his name was Frank. My eyes kept darting between Frank’s and the rearview mirror he was periodically checking. James was huffing short, panicked breaths. I’d wondered if he needed his meds.

We drove in silence for a while. You couldn’t see anything beyond the headlight’s path, just a deep empty void. The old man tried to spark up conversation, but neither James nor I were up for it. He’d asked if we wanted the radio on or off, if we were hungry, if we were cold, hot. Each time, I told him we were fine. He took the hint, and we drove for hours down the same stretch of highway having barely spoken. Until James had fallen asleep.

“I know it ain’t my business, young lady, but are you sure you and your friend are okay?” Frank kept his voice just above the hum of the radio. I assured him once more that we were fine, even though my mind was still back on Lincoln Ave, wondering what had been happening that very moment at the Murray household. If the flames revealed the truth below, or if they were extinguished before they got the chance. I played an imagined scene in my mind over and over: the fire trucks, the inspection of the damage, Paul watching eagerly nearby, ready to run. The discovery of bones, the call to the Sheriff, the arrest of the man he’d known and tried to help all those years ago, or whom he might now have to hunt down.

“Will you at least tell me your real name?” Frank asked, bringing me back to reality. We had so clearly been withholding truth from this poor man. All he wanted was just a small piece of it, maybe so he could justify the crazy thing he had done that day.

I looked down at my fidgeting hands and noticed the purple bracelet still tight along my wrist, the pink lettering of Mikayla’s name flashing with every passing street light. I’d forgotten that I was still wearing it. I thought about how badly I wanted to give her the ending she’d deserved. The one she’d wanted for herself. An escape. Freedom. How easy it would have been to do it, to say her name.

“It’s Rachel,” I uttered instead.

Frank smiled at me. “Well, Rachel. It’s nice to finally meet you.”

I let him drive us another hour. It was almost midnight. When I woke James to get out, he jumped. I had Frank drop us off at a cheap bed and breakfast, something I’d grown quite accustomed to over the years. I tried once more to pay him but he wound up giving me money instead. It wasn’t much, but the gesture alone was beyond kind. In spite of everything I’d been through that day and all that came before it, it wasn’t any less meaningful coming across someone as genuinely good as that man.

I felt bad that I’d lied to him about my name again.

———

James and I shared a bed, sleeping head to toe beside a rattling air conditioner. I wouldn’t have slept anyway. I was plenty happy with the four hours I got. When I woke early the following morning, I stepped outside for a cigarette and enjoyed every moment of it. I’d quit another day. James was sitting up in bed by the time I went back inside, his hair an awful mess, his tired eyes red. He’d asked me what our plan was. He was impatient, and I understood. I told him that we should eat breakfast first and figure it out from there. It had almost been an entire day since I’d eaten last.

When we entered the dining area, we saw that there were only a few other guests inside. I still wanted a table in back but James insisted we sit by the bar where a TV was playing the news. I gave in. He was worried about his mother, and I couldn’t blame him for that. I’d have been worried about mine too if she were still alive. I was really hoping this aspect of our lives remained different.

James was glued to the TV, even as the waitress came and took our order.

“You’re going to drive yourself crazy,” I told him as she walked away. He shook his head at me and kept his eyes fixed. We sat in silence as we waited for our food, and potential news.

“Your coat,” James suddenly recalled. “You left your coat at my house!”

I laughed, to which James looked bewildered. “It’s not my coat,” I explained.

“Whose is it?”

“Some guy named Scott, I think. Maybe Jordan.”

It wasn’t long before the waitress arrived with our meals. We‘d ordered the same thing, only my eggs were scrambled. There was something about the smell of bacon and home fries that brought comfort strong enough to make you forget that you were on the run. I moaned at the first bite. Probably could have eaten both plates. I even thought I saw a moment of calm in James’ face as he ate.

The TV caught our attention.

“Thank you, John. Authorities say they responded early last night to a fire in one very familiar Indiana home. The home of Mikayla Murray.”

James nearly fell out of his seat. I dropped my fork and a home fry fell on the floor.

“…Missing since 2008, Mikayla’s disappearance was one that rocked the small town of Millersburg, Indiana, but left many hopeful that she was still out there, listening. But when authorities found her car abandoned near the Elkhart River just miles from her home, friends and family began to fear the worst. Mikayla was gone, her whereabouts never discovered. Until now.”

I wanted to turn back to James but was afraid of the look on his face.

“When authorities cleared the scene last night at 1108 Lincoln Avenue, they made a shocking discovery that would answer a decade-long mystery, but spark a new one.”

It cut to the sheriff’s press conference. He spoke matter-of-factly while cameras clicked all around him. “The fire department responded to a 9-1-1 call around 5 PM last night. There was a gazebo on fire in the yard of the Murray residence, and when we assessed the damage, we discovered a bunker hidden underneath. Upon further inspection of the bunker, we found the body of a young woman and child. We’ve indeed confirmed the woman to be Mikayla Murray, but have no further information at this time.”

James squealed. “They found her?”

I ignored him, my face sunken. Waitresses and patrons were noticing our panicked state. Something wasn’t right. She’d been buried under there for so long, there shouldn’t have been much to find. And a child?

”It is believed that Mikayla had been held captive inside the bunker since that fateful day twelve years ago. Until last night when, tragically, both she and the child suffered fatal smoke inhalation resulting from the fire. Authorities have yet to confirm the identity of the child, or who started the fire. Mikayla’s mother, Linda, is being questioned by police while federal officials search for her father, Paul, and brother, James, both of whom are now missing. If you have any information on their whereabouts, please call this number, and stay tuned for more on this story...”

I couldn’t feel my body. I turned around and stared down at my shaking hands on the table, the world caving in on me.

“What happened?” James cried. His breathing was heavy, his eyes bulging out of their sockets, staring at me, bewildered.

I finally looked at him. “I killed Mikayla.”
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "49e9dbca-b7c0-4852-987d-73c265ee0b3f",
                    ThreadTypeId = 1,
                    Title = "The beginning of a story",
                    Content = @"
Right now, the car is headed silent down the highway. It's dark, and there is nobody driving. I snuggle up in my seat and listen to the hum of its parts. I have turned my set off. It shows nothing but reports of destruction and plagues. The world on fire. The world gone mad.

Most of the interstates have shut down. They want people to stay in one place. The car is moving along the back roads, switching from one lonely little highway to another. We are headed towards the answer, towards the key to defeating Q. I hope we get there fast.

Slowly, the sky pales, and the blue curves of the mountains emerge from the darkness beyond the guardrails. I heard once that the Appalachians used to be as high as the Himalayas. Looking at the sloping hills under the sky, I can sense the ancient shape of the world. A world that was here before us.

Man, I'm getting pretty philosophical.

In my mind, another shape appears. Massive. Continental. The slope of human decline. The awful descent of the human race into...

Christ. Let's just enjoy the pretty mountains.

Karen is lying in the back. She's doing another eye treatment with equipment we took from the hospital. Before we reach Plattsburgh, the car switches highways and heads west. The sun climbs higher. We are getting closer.

Eventually, the car turns onto an unpaved road. After few minutes, it slows to a stop. And here we are. I look around. It's a nice bit of country scenery -- grass and trees and gentle hills and blue sky and pretty much fuck all. There is nothing here. Or whatever is here, is hidden.

Karen is still doing the eye treatment in the darkness of the van's rear. The light from the goggles seeps out in little flashes, sketching the shape of her face. Finally, the goggles turn green, and she pulls them off, blinking and squinting.

I go and help her sit up. ""Can you see a little better?"" I ask.

She looks down at her hands, moving the fingers slowly in the dark. ""Yeah.""

""Persistent shapes?""

She raises her hand into a shaft of sunlight shining in from the front of the van. Her fingers catch the glow. ""My hands,"" she says softly, her voice quavering with disbelief. It's the first strong emotion I've ever heard from her.

""Good. That's great,"" I say. ""Well... we're here. What do we do now?""

She looks at me and smiles maniacally. ""We go into the forest,"" she says. Her smile is unnatural and stiff, more of a grimace than a smile, but for a brief moment, as it first spreads across her face, she looks like a giddy little kid. ""The key is there,"" she says.

""What is it? Some kind of secret underground base? Hidden laboratory?""

She makes a groaning sound that I barely recognize as laughter. ""You play too many narratives. It's much simpler than that.""

I unfold a wheelchair that we ""borrowed"" from the hospital and help her into it. When I open the back doors of the van, she winces against the bright sunlight, and again her face looks like a little kid's for a moment. I give her a pair of huge black wraparound sunglasses that we took eye treatment center.

The van lowers to the ground, and I roll the wheelchair out onto the dusty road. She makes sure I take a bag of supplies with us -- snacks and drinks and other stuff. The sun is warm on my skin, but the breeze is fresh and cool. It's a perfect day. You would think that everything is right in the world.

""So where to?"" I ask.

She looks around, her head wobbling on her thin stalk of a neck, her eyes hidden by the massive glasses. ""There was once a house here. Do you see it?""

I look around and spy a low, crumbled gray wall mostly hidden behind the high grass. ""I think see an old foundation.""

""That's it, she says. Her eyes are hidden, but there is something in her voice that wasn't there yesterday, a shivery excitement. It makes me excited too. I push the wheelchair down a weedy gravel driveway toward the foundation. There's nothing else left of the house. It must have been torn down and hauled off. Karen has me push her around it and go down a trail leading towards forest.

""What was that house?"" I ask. ""Anything important?""

""I used to live there.""

I turn and give it another look, as if I would see some new detail in the crumbling concrete that I had missed.

""That was the old children's home?""

""Yep.""

""Then where are we going?""

""We're almost there,"" she says. ""It's close.""

We follow the trail into the forest. The trees become thick and shadowy. The wheelchair has a little power assist, but it's still tough to push it over all the roots and rocks and that lie along the narrowing, twisting path.

""Oh, yes!"" Karen whispers excitedly.

Up ahead, sunlight gleams through the branches of the crowding trees. A wave of excitement moves through me, and I push Karen faster. We come out into a clearing, a broad patch of wild grass that glows green and golden in the sunlight.

""Here,"" Karen says.

I stop the wheelchair and look around. At first glance, there doesn't seem to be anything here.

""So what's here?"" I ask.

""I used to come here as a child... and play make-believe... before I was connected.""

I take a walk around the clearing, looking for something. A hatch? A hole? An actual key lying in the grass? There is nothing.

Across the clearing, Karen is slowly pulling off her sunglasses. When her eyes appear, they startle me. They are wide and gleaming within utter fascination. I walk up to her. She is staring at something. Tears fill the rims of her eyes and spill over. What is she looking at? It seems to be something right in front of her, something I can't see.

I stand beside her and crouch so I can see what she is seeing. There is nothing there but a small cloud of gnats. ""What are you looking at?"" I ask.

She looks all around and takes a deep breath and shudders. ""There's... more..."" she whispers.

""More what?""

""They said the feeds were complete... but they were wrong.""

I wait for her to say more, but she doesn't. ""What do you mean?"" I ask.

She looks at me and smiles, the most goofy, crazed smile I've ever seen, tears still flowing down her cheeks. ""The designers of the feeds said that it provides a complete experience. Enough colors, enough frames, enough smell gradients, enough complexity to make it indistinguishable from reality... but they were wrong. Here! Look at them!"" she says, raising her hand into the air.

""You mean... the gnats?""

""Yes.""

The gnats are glowing specks dancing senselessly in the sunlight. I wonder if some pattern will emerge. Can Karen control them with their mind? Is that the secret? Are they forming shapes? But they just dance and dance, forming nothing, making no pattern that I can see. I feel silly for even thinking that they would. They're gnats.

I turn away. A flood of angry thoughts rushes through my mind. Gnats? Fucking gnats? She's a nut. She's lost it. Yeah, she's powerful and impressive in the feedrealm, but now she is in the real world, and she has completely lost her shit, and this whole trip has been a waste. ""Is there anything here?"" I ask. ""What's the key? Seriously. Don't give me any of that bullshit like 'I can't explain' or 'You'll see.' Just tell me. What are we doing here. What is the plan?"" I ask, almost shouting by the end.

The crazed look of joy fades from her face and is replaced by the look of a scolded child. She lets her head hang and wipes the tears from her face with her weak little hands.

I feel a bad. I kneel by her chair and say, ""I'm sorry. Please, just tell me what your plan is. I need to know now.""

Karen begins speaking softly without looking up. ""Q has base control of every major system in the world. Every drone, every rover, every defense robot, all orbital assets, all nuclear weaponry. She has control over most human political systems. She has destroyed or contained every existing countermeasure, including me. There is no scenario in which we could ever reacquire control. Not with a thousand times our current resources. Not with a thousand years of computation time.""

""So then what's the plan?""

""What we need is a way for Q to be destroyed by just one or a few motivated individuals. I believe there were points in the past when this could have happened. Maybe one of the Germans overseeing the early research program could have stopped it. Maybe it could have been stopped around 2020, when the portals were shut down, and interface research was temporarily abandoned. But it didn't happen. Currently, at this point, there is no way for it to happen. Q has control of far, far too many assets. The war is already lost. Irrevocably.""

""Then what do we do?""

""We must hope that there are alternate timelines and that somebody in one of these timelines foresees what is happening to us right now -- that somebody foresees this very moment in time and takes steps to prevent it.""

I stare at her. She looks into my eyes. I grope for words. ""Is that... Wait... Alternate timelines? Is that the plan? We have to send a message back into the past?""

""In a sense.""

""Then the person who receives this message will destroy Q in the past, and that will save us?""

Karen shakes her head slowly. ""No. That clearly won't happen or everything would already be different. We are utterly doomed. We'll either be either incinerated in a nuclear strike or rounded up and incorporated into Q. There's no stopping that. The only hope to defeat Q is on some other timeline, if such a thing exists.""

""There's no hope for us? At all? Then what are we doing here? Why are we in this fucking clearing?""

""Haven't you felt it?""

""Felt what?""

""The feeling that you're inside a narrative.""

An eerie shiver comes over me. I look around at the clearing. ""Like, I'm inside a feed?""

""No. Inside a narrative. A story in somebody's mind. Doesn't this all seem just like a story? Two people rushing off to save the world, to find some hidden key in the forest?""

""Yeah, it all seems pretty unbelievable.""

""That's how I wanted it to feel. That's why we came out here. So that we can be inside a story. Now, hopefully, there is somebody out there in the past who will write the story.""

""Write the story? What? So there's nothing here?""

""There's no magic key or secret underground base.""

""Well this story sucks.""

""Why?""

""It's a huge fucking let-down.""

Karen makes a mild choking sound that might be a chuckle.

I slump down into the grass beside her wheel chair and hang my head. I'm out in the woods with a crazy person. She doesn't even make sense. She's spent too long in 5D. She's talking about alternate timelines. Finally, I ask her, ""So we're just fucked, right?""

""If you look toward our future, if you look at the series of events which will happen to us, they are dark. They are very awful. We will suffer. We will die. But that would be true in any timeline. On the other hand, if you look at the entire story, not as a series of events, not from beginning to end, but as a single continuous, connected shape, where every event is occurring simultaneously... I think... my life... even my stupid little life, which I spent mostly inside that hygiene bed... could form a beautiful shape.""

I snort. I'm tired of this cryptic bullshit.

Karen goes on. ""Maybe that shape reaches back, back to some place where somebody can see it and change things.""

I don't say anything. Karen reaches into our bag of supplies and pulls out one of the little paper notebooks she bought at the gas station.

""What are you doing?"" I ask.

""I'm going to write a poem. Do you want a notebook?""

""What for?""

""Maybe there's somebody out there who needs you to write a story.""

""Who would read it? Isn't everybody going to die?""

""Who knows,"" she says and drops the other notebook into my lap. ""Maybe somebody would be interested.""

I toss the notebook off into the grass. Fucking pointless. I can barely write on paper anyways.

We sit in silence for a long time. When I look up, Karen is staring at that same little cloud of gnats, occasionally jotting stuff down. I find myself staring at them too. They look like nothing more than living specks of dust worked into a crazy, whirling frenzy. Is there any pattern in how they move? Would it matter if there was? I think about what Karen said about the shape of her life, what it would look like if everything happened simultaneously, if it could all be seen at once. I think about the shape of my own life. I stare at the gnats and imagine seeing every position of every gnat all at one time. What kind of shape would it make? Even if I could see it, would this shape have any meaning?

I pick up the notebook and begin to write.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "1894e044-dd59-4000-8865-6804b13c8cd5",
                    ThreadTypeId = 1,
                    Title = "The Hayloft",
                    Content = @"
I like mornin’ best in summer. Chet don’t, cuz he’s fifteen and likes his sleep, but I’d rather make my own breakfast, get chores done quick – enjoy the rest of the day if Mama gives me a choice. Let Chet take the afternoon. He thinks he’s winnin’, but he ain’t. Molly moves a lot during late milkin’, sheep and horses need rounding up. It’s harder and dang hotter, and more flies – that’s a fact.

Sun’s not close to bein’ up, but the air is cool and fresh, not too wet, and the sweet scent of lilacs fills the barn when I prop them big doors open. I breathe deep. Molly kicks a leg back, swishes her tail as I shift the stool and nudge the bucket back. I give her smooth tan flank a pat, “Whoa there, Girl,” and go back to milkin’. I got a weird feeling. Look for Roscoe out the doors, but he ain’t there – dumb dog. There’s a flicker in the floodlight circle outside. I gasp, feelin’ off. Sometimes I spot a Luna moth in the light all glowy-green and big. They don’t stay long, too smart. Bats chase ‘em, but I ain’t never seen a bat catch a Luna moth. I might like that, I think, maybe.

I hear a noise come from the hayloft above. Molly moos high-pitch like, shakes her head, jinglin’ her bell. I got goosebumps. She don’t ever do that. My eyes shoot up at another creak; somethin’ with weight by the hay door. I can’t see nothin’ through them slats – could be rats nestin’, maybe. I smell somethin’ too, somethin’ foul mixin’ with them lilacs.

The hayloft is a sanctuary – Mama says, in the strong light of day. I like readin’ up there, propped on a bale near the hay door in the afternoon when the breeze is light and I can see all the way to the lake – but now, since Daddy… “Odd, no rooster crowin’ – ‘bout time, ain’t it?”

Molly don’t answer. There’s another flicker, a shadow in the floodlight circle like somethin’ peeked out the dang hay door above. I squeeze the teat too hard, eyes on the circle of light – waitin’, thinkin’ ‘bout what’s up there, and that weird smell, ‘bout no rooster. I suck in a hard breath, cough a little. Molly stomps her hind leg hard. I flinch. Milk sloshes out the half-full bucket side, splashes my rubber boot. I shake my head. If he were here, Daddy would say, “If you ain’t thinkin’ about what you’re doin’ then you ain’t doin’ what you think.”

I smirk – go back to milkin’ in lamplight meant for baby chicks. Ain’t no eggs hatchin’ right now nowhere, so I prop that light to where need be. Daddy always said I was best milker – don’t need no light, but baby chick lamp is warm and not too bright. Chet don’t care enough – that’s why I’m better. Molly moos again. My ears perk. Its peaceful most mornin’s less them chickens get a rat in the coop, or somethin’.

Overheads are so bright, and I hate flippin’ that giant switch outside that takes two hands, and makes a sizzling noise – no thank you. No sense in wakin’ the whole barn, or rilin’ up Daddy’s old hound Roscoe – gawd, he must be old as Chet. All wiry and gray fur that felt like pettin’ a bristle brush the few times he let me. Dog never did warm to me, even when Daddy was alive, but used to wag his tail, at least. Now, he just kind of follows me, watchin’, lookin’ all mean. Sometimes he barks at nothin’. I think his hearin’s gone bad, or maybe he plain misses Daddy, but I got used to him. Funny, him not here now, growlin’ at the wall.

Overheads do no good in the hayloft; only daylight. I don’t wanna in the dark, but soon I gotta take my boots off, climb that old rickety ladder and toss down three bales for the horses – too quiet, I notice, not even a whinny from Taffy. I think about the shadow again. What if there’s a raccoon up there – or a possum? They can be awful mean. Where is that Roscoe?

Daddy always hated that loft in dark too. Strange, how Chet found him up there. His face… Doc said was a heart attack. Chet weren’t himself – cryin’ an all. Told me Daddy looked scared. Thought he were still alive cuz that look on his face. I can’t never picture it – Daddy scared? Suits me just fine that I can’t.

Two years he’s been gone now. We weren’t never close. “No use for girls,” he’d say, but he’d wink when he said it. He liked that I weren’t no whiner, even when Molly stomped my foot, broke my little toe. I weren’t mad – was Roscoe spooked her, barkin’ like that. Sometimes I like Molly better than Sara from school – talk to her more – that’s a fact. Them heavy creaks above sound to me like footsteps shufflin’. Molly turns her head back toward me, glaring with that big saucer eye. I think, can she sense my fear?

“Riitttaaa,” something whispers from the hayloft. I don’t breathe. Can’t be, I think. Sounds like Daddy. No breeze to rustle hay. Horses are quiet. I stop milkin’, pat Molly’s flank again, and press up to her side. I rise up off that old stool real slow, linger a moment. I feel safe there nuzzled to her soft hide, start breathin’ again till I hear it.

“Riitttaaa,” it moans louder than before.

I’d think it Chet playin’ a trick on me, but he ain’t funny – not never. I wish Roscoe was watchin’ me, lookin’ mean, growlin’ at the wall. I close my eyes, promise to be nicer to that dang hound if he just appears right now. I open ‘em wide, but no Roscoe. I force my feet to move toward the pump sink. “Riiitttaaa,” it repeats, sendin’ chills all through me, soundin’ so much like Daddy. There’s a flashlight on a hook. I feel around, find it – click it on. It flickers like that shadow did. I tap it to my palm till it shines bright, let out a long breath then go check the horse stalls first.

Taffy likes a handful of sweet grain in mornin’ – never missed, strange, her not neighin’. I don’t hear no crickets, neither. That quiet is raisin’ them hairs on the back my neck to run all the way down my arms. I stand on tiptoes, point that light over, and hold my breath again. Ain’t no horse there. Looks like Taffy done kicked her stall open. Latch is busted. She must be far in the field if I can’t hear her whinny. I breathe out, think: how’d she get past the gate? Chet couldah left it open, I suppose. He gets lazy toward supper time.

“Riiitttaaa,” it calls from above. I pay no mind thinkin’ it can’t be real. Check the next stall. Traveler’s done the same dang thing. Now Traveler couldah jumped that gate in a flash – that’s how he got his name; gallopin’ all the way end a town ‘fore Daddy caught him. I need to get Chet, I think, but point the light through the stall to the wide pen beyond – see if Taffy and Traveler are out near them sheep. Two dozen beady eyes stare back – no horses. A few sheep bah at the light that don’t go past ‘em. The flashlight goes out. I shake it. It comes back, but weak, shinin’ on blood spread all over that mucky third stall floor – opened from the outside, it looks. And like somethin’ were dragged through the hay, out ‘round the corner. “Kettle Jack,” I whisper.

“Riiitttaaa,” It’s callin’ me, still. I’m tryin’ to ignore that whispery voice, like them stories Daddy told of the Huggawuggahs to scare me. I think, so I wouldn’t run off visit Sara in town two and a quarter miles away. Them dark miles with no street lamps, but I weren’t scared, not really. Only once – was nine – right before Daddy died.
He caught me halfway, pullin’ up in that old truck well after eight, and boy, was he mad. I’s glad to see him, though. It was real dark. He told me about them Huggawuggahs on the way back. I giggled then got quiet. Daddy looked real serious at me. “I know there’s a one livin’ other side of the lake” he said, “…ain’t never caught but shadows and whispers, but I know. You best be careful – them is tricksters. Chief – you ‘member sold me Kettle Jack?” I nodded. “…he seen one. Scared him silly and he’s toughest man these parts, ‘sides your daddy. Don’t you go walkin’ past dark, not ever.” He said the lake Huggawuggah kept to itself less it got too hungry, I ‘member that. Did Daddy say he left a sheep or two in winter by wood’s edge? Or maybe that dang Huggawuggah just took ‘em. I can’t recall.

The shufflin’s almost right above my head near the cutout in the loft to push bales through. And that foul odor gettin’ worse. “Riiitttaaa,” it cries, soundin’ an awful lot like Daddy. I point the flashlight at the rectangle hole, feelin’ brave for a second. It flickers and goes out, but not before I glimpse what looks like – can’t be – my daddy hunched over that hay hole. I freeze, drop the light, find it hard to breathe. I shut my eyes tight.

Didn’t Daddy say Huggawuggahs could change shape? Maybe that’s one up there now. Maybe it weren’t no story. “Riiitttaaa,” It calls, won’t quit. I stumble back; kick the flashlight – dang boots. Open up my eyes, then pick it up and shake it. The flashlight’s out for good. I back up to the pump sink, feel the wall in the dark for the shotgun wedged in its spot. Don’t no one ever touch it less need be – loaded is how Daddy kept it. “Can’t shoot nothin’ with an empty gun,” He’d say.

My fingers brush the cold metal, thinkin’ back to if Chet fired it since Daddy died. “Riittaaa,” it calls from above. I wipe my hand gone all sweaty, on my jeans. As quiet as I can, I pull the old shotgun from the wall; run my hand along the rib, the barrel. It’s heavy, but I lift it till my fingers flick the safety off. With me shakin’ so bad I need keep that shotgun close. “Riittaaa,” it pleads, like it needs me. I hug Daddy’s shotgun to my body, cold barrel pressed to my cheek. Only one shell – one shot, I think. I look to the door. “Roscoe,” I whisper, and wait a tick but ain’t nothin’ but the light circle and the dark all around. I bite my lip; swallow hard, hope that weren’t Roscoe’s blood in Kettle Jack’s stall… Hope it weren’t Kettle Jack’s neither.

Molly’s mooin’ something awful. I tiptoe back near her. Hard in them dang rubber boots, huggin’ Daddy’s shotgun, to run a hand along her side, give her a pat. She kicks her leg back, shuffles her hind away like she don’t want me touchin’ her – I must be oozin’ fear, I think. I glance out them open doors to the circle of light, prayin’ for that old dog to waddle in and watch me. It’s gray outside, still moonlit, but getting lighter. “Riiitttaaa,” it wales louder than Molly mooin.’ Could make a run for the house, but Molly – and it’s still dark, and what if there’s a Huggawuggah out there?

In my mind I see Daddy shaking his head. I toughen up. I ain’t no whiner. I walk close to the hole, take a deep breath. “Riiitttaaa,” it calls so loud it hurts my ears, and my eyes are waterin’ from that nasty smell. I do it quick. Aim the shotgun up that hole at the shadow and pull the trigger, like he taught. Bam!

It kicks me back, but I don’t fall, just hit Taffy’s door. I hear the milk bucket spill. Somethin’ shuffles above then staggers through that old hay hole – hits the ground hard, front of me. My heart’s beatin’ so fast. In the gray light, that shape curled up too close to my feet looks like a man – then not. I don’t move an inch; press harder to Taffy’s door, breakfast climbin’ back up my throat. I can hear shallow breathin’ a minute or two. “Riiitttaaa,” it hisses… then nothin’.

Outside, past the floodlight circle, a flashlight beam zigzags toward the door. Chet is there, out of breath. I want to hug him. He’s in sleep pants, no shirt, looking real scared.

“Rita – what-in-the-hell?” he asks, his eyes so big.

I think that’s what Daddy’s face mustah looked like, then wish I didn’t think it. Chet aims the beam over where it fell, to the shape. The light shines on Daddy’s old hound dog Roscoe; lyin’ on his side, blood oozin’ from a hole ‘tween his ribcage, deader than dead, and stinkin’ like he done died last week. His fur looks darker, softer, and his face looks different too – bigger teeth, wider eyes like a wolf. Chet don’t seem to notice. He pries that shotgun from my tight grip, pats my head.

“He – Horses are gone. There’s blood in Kettle Jack’ stall. Was scared – just shot,” I manage to say, still shaking somethin’ awful.

Chet runs back out, flips the big switch. Gawd, it’s bright. He checks outside then Kettle Jack’s stall. “Horses by the road – they fine. There’s a sheep down, mauled awful – dead,” he says. I let out a long breath, swallow breakfast back down. Chet reaches way up over the pump sink, fetches another shell to load Daddy’s shotgun. Then he nudges Roscoe’s side with the barrel, sees blood ‘round his mouth, on them big teeth. He hugs me quick, “You never did like that dog none.”

Mama comes in next. Boy, does she look affright. “Rita, Baby, what-in-the-”

She loses her voice as her eyes fall on Roscoe. She hugs me to her breast so tight. I feel safe there, like with Molly. She whispers, “You never did care for that dog, much.”

“I did,” I say, “I’m sorry,” but I’m not. Roscoe couldn’t a climbed up that rickety ladder, no how. Weren’t him I killed. Was a Huggawuggah – that’s a fact. Even if I couldn’t never tell – I know. Molly knows. Just then Molly moos sweet like she agrees. I call out, still clingin’ to Mama, “Whoa there, Girl.”

“Mustah gone mad – rabies, maybe – you bit, Baby?” Mama asks, squeezin’ tighter, like she’s trying to crush the fear right out of me. I shake my head no; start to say somethin’ then stop. I don’t tell Mama or Chet about it being in the hayloft, or it looking like Daddy, or about it callin’ my name over and over. Chet and Mama don’t believe in no Huggawuggahs.

I miss my Daddy somethin’ awful then… thinkin’ now, maybe we was close. That he done saved me by telling me about them Huggawuggahs before. I squeeze Mama tight, pretend its Daddy. Tears leak out. Maybe that lake one killed Daddy – scared him dead.

Molly moos again. I think about that tipped bucket – I ain’t never tipped one before, not ever, not even when Molly broke my little toe. Daddy always said I was best milker. Laughed, I ‘member, nicknamed me cuz I filled that bucket so dang fast. I knew it never were Daddy what called out ‘Rita’ from the hayloft. Since I’s six years old Daddy always called me ‘Squirt.’
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "b0194a3c-711b-4aff-93f1-8ecaaef7d389",
                    ThreadTypeId = 1,
                    Title = "A Crack in the Dam",
                    Content = @"
Leo Banks drove furiously down the road towards town. His mind raced as he considered the implications of what he’d just seen and just how he was going to warn everyone. The sun was setting now and by the time he would get there it would be nightfall. It was Halloween. Kids would be everywhere. The fire department would be kicking off its annual Trunk or Treat festivities along Main Street. He didn’t want to create chaos. But who? Who should he go to in order to get the word out that the dam, the giant Gilboa Dam was going to break and flood the entire town?

A member of the fire department. They would be around. He could go to one of them first, maybe suggest an orderly evacuation. He’d have to pull someone aside from the festivities which would surely cause a scene. Maybe that wasn’t the best approach. If someone were to overhear him it could cause the very mayhem he was trying to avoid. Bedlam on top of an impending flood wasn’t what he wanted. He wanted to find the right person or organization to appropriately deal with the impending disaster.

Time was of the essence and he would need to act fast. He felt a strange but brief surge of gratitude at having been granted the opportunity to have this moment of heroism. The universe had put Leo in a unique position, and now an entire community could be saved by him. Not many were given such a chance in life.

As he pulled closer to town, he could see the children running around in their costumes. The evening had begun. His sense of heroism began to ebb and change back into a panic. He thought of all of the poor souls, unaware of the danger they were in.

He pulled to the edge of town, just before the road was blocked off for the festivities. He parked his car crookedly and leapt out. His time was now. He felt the sense of heroism return.

“Sir, you can’t park there.” A woman called out to him.

Leo spun around confused.

“I’m sorry?” he said.

“You can’t park there. We have to keep this area clear.”

“This whole town needs to be cleared.”

“Excuse me?”

Leo approached the woman and inspected her. He studied her reflective vest and flashlight as he looked her up and down, assessing her potential to help.

“Are you of some stature? In the fire department?” Leo asked.

“I’m….here as a volunteer.” The baffled woman replied.

Leo nodded as he turned again towards town, leaving the woman and his hastily parked car behind him.

“Sir, your car!” the woman cried out to no avail. She sighed as she watched Leo walk away.

He found himself in the thick of the crowd. Throngs of children and adults in costumes surrounded him while the Halloween music blared from a nearby truck, fully decorated as a ghoulish graveyard with tombstones, skeletons and cobwebs. The music combined with the shouts and laughter of the disguised children filled the air with a dizzying cacophony. Overwhelmed by what may soon happen to all of these people, he frantically continued his search.

A police car was parked by the old clock in front of the town hall. Leo ran towards it. He considered what a shame it was that something like the old clock, a piece of history, would likely be destroyed soon. The lights on the police car came on and he paused. Three children in costumes emerged from inside the car and a smiling police officer gave them each candy as they exited. What was this? How could he approach this officer with such a grave matter while children stood by? Maybe he could ask to speak with him in the car alone. Disappointed, he decided against it and pressed on. If the officer wanted to engage in such frivolity, maybe sharing the heroism wasn’t for him.

It all began earlier that evening. With only the faintest of October sun remaining, Leo was returning from an afternoon hike. The same hike he took every Thursday after work. The trail’s final bend before emptying back out to the parking lot offered a view of the massive dam. The water behind it supplied the drinking water to New York City. A city three hours away. It stood as a hovering threat to those that lived in the Schoharie valley. Many residents knew the exact time frame they had to evacuate their homes should the dam ever break. In some cases, it was as little as seven minutes. The entire valley would fill up for miles.

All of this only fed the panic that Leo felt upon seeing a man (he was certain he saw a man) placing a blinking device on the dam. Leo stopped and tried to get a closer look. The shadowy man caught sight of Leo. He flipped him off and disappeared into the falling darkness. Leo, unsure of what to do, began to run. Were there officials working at the dam at this hour? Should he try to remove the device? What would he do with it? Was there time enough to even consider these options?

He decided he would first try to get the device off. He slid down the embankment through the brush and hurried across the base of the enormous dam to where he’d seen the man. He climbed up the opposite embankment and to the location of the blinking device. As he came face to face with it, he realized he had no idea its potential. If he were to remove it, where could he go? If he brought it into town for someone to deactivate, a specialist of some kind, he’d still be putting everyone in danger. The clock on the device read thirty-eight minutes and counting. He tried giving it a pull. It was fastened tightly. There was no time to waste.

Leo ran back to his car and within minutes had the speedometer pegged at 88 m.p.h. The dam was going to break. The burden was his alone to bear. He drove northeast, out of the fading light and into the falling darkness.

Now he stood shaking in the middle of Main St., unsure of where to turn next. A child in a grim reaper costume bumped into him. Looking up at Leo, the mask lit up in blood red. The mother laughed and excused her child. Who were these people? What excuse was there for such nonsense?

Leo tried to collect himself. How much time was left? Why hadn’t he thought to keep track? There was no other choice but to return to the frivolous peace officer and alert him to the impending disaster.

More children were emerging from the patrol car as Leo approached. The officer laughed and gave the kids candy as they continued on to the next car. Now was the time.

Leo approached.

“Officer? A word please?” Leo spoke in a hurried but hushed tone.

“How can I help?” the officer smiled at him.

“This is of grave importance. I need your full attention.” Leo waved his hand towards the candy and the car.

The officer set the candy down on the car, waved to his partner to take over and the two stepped away from the crowds and stood beneath the clock tower.

If someone were to look over, they would have seen a man gesticulating wildly as he pointed towards the direction of the dam and towards the crowds around them. The officer listened grimly but revealed no sense of urgency. As he spoke into his radio, Leo dropped his shoulders in disbelief.

“What don’t you understand?! This has to happen now! These people need to be evacuated!!” Leo’s voice grew louder. Many passersby took notice of him and some stopped to listen. The clock tower chimed seven o’clock.

“Listen, bud. I put in a call for someone to go and check it out….”

“There. Is. No. Time! Why don’t you understand?!”

More parents and children had now gathered around the two of them.

“Look, you’re scaring people for no reason, pal. We’re checking it out. If you don’t knock it off, I’ll have you detained. Got it?”

Leo stood in disbelief as the officer returned to his car.

The music that had been emanating from large speakers on a flatbed had stopped. Feedback screeched from a live microphone as the village mayor, rotund in appearance and dressed as a giant pig man with a monocle and top hat, tapped the mic as he stood amidst the gravestones decorating the truck.

“Check, check. Hello?”

More screeching from the microphone. Many in the crowd winced as they covered their ears. Leo looked around as the crowd began to congregate around the makeshift stage.

“Alright, well, hello everyone! Happy Halloween!” the mayor exclaimed. “Who’s ready for the costume contest?”

The children began to cheer as their parents applauded. Leo, his eyes frantic and wild, shoved a child out of the way and rushed towards the stage. Knocking into confused, costumed children and their protective parents along the way.

“If everyone would just line up over here….” The mayor continued.

Leo leapt up onto the flatbed. A look of shock overtook the mayor’s face.

“Excuse me!” the mayor tried to back away from the crazed Leo. “What are you doing?”

“Give me the mic.” Leo demanded as he confronted the mayor, grabbing at the microphone.

Leo and the mayor fought for the microphone, with Leo emerging as the victor as he snatched it from the mayor’s hand and turned to face the confused and concerned crowd.

“Everyone! Listen up! There is no time to waste! You need to listen to me. The dam is going to break. Do you hear me? Do you understand? The dam is about to break! We need to evacuate now!”

Several members of the crowd gasped as the mayor fought with Leo to get the microphone back. Leo kept him at arm’s length and continued on with his warning.
The police officer shook his head, set down his candy bowl and ran across the street towards Leo.

“Please listen to me!!”

The officer hurried up the steps of the flatbed and moved past the flustered mayor and towards Leo.

“Please! For the love of god get your children out of the valley now! Go….”

“Alright pal c’mon” the police officer intervened.

The officer was able to wrestle the mic away from Leo. He handed it back to the mayor and led Leo off of the flatbed and away from the crowd.

“Alright everyone, let’s just settle down.” The mayor pleaded. “Everything is under control. Please don’t worry yourselves.”

The officer had Leo against a building and had just begun to handcuff him when something made them take pause. A deep rumble echoed though the valley, causing an eerie silence amongst the crowd. Everyone stopped what they were doing. Leo’s attention was drawn towards the old clock across the street. Standing there below it a familiar, shadowy looking man, only slightly illuminated by the overhead streetlight. The man smiled, flipped Leo off and then receded into the darkness.
A woman let out a scream and pointed down the road. The first sign of water had come into view. It moved with the force of a charging army towards the crowd. The officer looked in disbelief at Leo.

“I told you.” Leo whispered.

The rumble became louder. The water began to flow faster and higher. Everyone scrambled and ran in the confusion. The officer turned his attention from Leo to the crowd.

“Head towards higher ground!” he yelled to anyone who would listen. Leo watched as the officer disappeared into the chaotic mess of frightened people and the surging floodwaters. His words dissolving into the noise that was engulfing them all.

Leo stood alone, with his hands still cuffed behind his back.

“I told you!” he screamed before he was knocked off his feet and engulfed by the unstoppable, indiscriminate force of water as it flooded out from a broken dam. The water showed no mercy as it crashed through Main Street and overtook the screams and panic within minutes.

When it was all said and done, the only survivors left were in the surrounding hills. Their homes now inaccessible from any road. They stood by powerless as bodies, fake gravestones, livestock and any manner of detritus floated around their elevated homes. For weeks, helicopters delivered supplies and even airlifted residents out as the entire valley surrounding them sat filled with water. Water that had refused to be held back any longer and now had no place left to go.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "57cf58c3-44a5-4a32-a83a-49b36d8bf7c6",
                    ThreadTypeId = 1,
                    Title = "They Only Take One",
                    Content = @"
Take in that fresh air girl! Damn it feels great to be back. Everything looks smaller though.

“Yeah, because you were like three feet tall last time you were here, ya walnut.” Buddy “Bud” Jake looked at Gwen with a smile. The classic platonic boy-girl relationship where no sexual tension existed. Mostly.

“Well, sorry I didn’t come out of the womb at a lengthy 5’9,’’ Gwen playfully threw her circle-k coffee cup at him. She knew it was empty, but Bud didn’t.

“What the .. oh real nice. You’re lucky that was empty.” “And don’t worry I’ll throw it out litterbug.” Bud made an exaggerated motion of throwing the coffee cup out, all while maintaining eye contact with Gwen.

“Ok, awkward.” “And yes, I have been an amazon since birth thank you. Good thing you finally grew into your shrimpy grade school body, so you don’t have to look up to me anymore.” Gwen smiled.

Bud and Gwen have been best friends for their entire conscious life. They grew up together, their families lived on the same street, they even played the same sports. Gwen made the little league baseball team and Bud became the first male cheerleader of their high school. You could see the absolute fireworks between the two when they cheered together.

“I’ll always look up to you Gwenny,” Bud said with a corny wink. “Glad I did get taller than you are though. That was a close one.” Gwen cracked a closed mouth, somewhat embarrassed smile as she looked at the ground. She missed Bud amazingly. But she would never admit that to him. No reason that she could think of, maybe a girl thing, maybe she just wanted to keep her emotions to herself. Maybe she never wanted to ruin anything by even remotely letting Bud know how much she cared about him.
“We’re wasting precious drinking time,” Gwen said, straightening up and pushing her feelings down. “Come on then Mr. Man, let’s blow this shit out like we were kids again.” Gwen looks over the lake park, cups her hands over her mouth and yells. “SHAKEY LAAAAAKES,” A little “woo,” followed as she turned back to Bud with an ear-to-ear grin. Bud was delighted. “Let’s get this party started,” Bud said, with another wink. This one wasn’t corny. More ominous if you had to categorize it.

Shakey Lakes, Michigan. Some towns have an amusement park. Some towns have a civic center with the community pool. Some towns have the world’s largest thermometer. Lake Hill, Michigan had the Shakey Lakes state park. Saying you were going to Shakey lakes meant you were going to the smallest campground in America. The series of lakes are many, but pretty small. The campground only had about 50 lots., and a just a handful more for those that wanted to camp for real. Like with a tent.
Even in the early and mid-90’s most people in the country wanted to feel like they were camping but wanted the modern convenience of home. Anyway. Bud and Gwen have not been here in many years and decided to get together after they graduated college.

“You are right about how small this place looks,” Gwen said. “It looked so massive when we were kids. The beach is like a sandbox!” Bud laughed a bit. “Don’t remember it this.. dirty either. Felt like it was cleaner when we were young.”

Bud nodded. “We didn’t notice if it was clean or not. We just wanted to race to the beach, put our towels down and start whipping water toys around. I see what you mean though, it does look kind of rough here. The volleyball nets are down, the parking lot looks like a bomb exploded, and the arcade used to be right here, but I don’t see it. Hopefully they just moved it.” “I’ll plug this bad boy in, and you start putting out those fancy dancy little lights out on the awning.”

“Please tell me you didn’t forget to grab that AstroTurf my parents left for us to use,” Gwen said with that pleading don’t let me down look in her eyes. Bud jumped into the RV, making obvious noises like he’s looking around. THIS ONE? He said, muffled, then threw the roll out of the door. Gwen let the breath go, relieved that he didn’t forget.

“Yes, that one,” she said.

Bud came barreling out of the door, down the tiny steps and onto the ground in front of Gwen and the AstroTurf. “You know never to doubt me,” he said smiling. “I told you I would be the first guy on the cheerleading squad, didn’t I?” “You did, team captain, you did,” Gwen said. “And thank God you did, no one else could have handled me like that.” There followed a long silence. They both spoke at the same time.

“Well, I,” Bud said, cut off by “You know wha..” Gwen interrupted. This happened once or twice more before they just laughed and shrugged everything off. “Let me plug this thing in,” Bud said.

Bud and Gwen were the same age. The exact same age, save for a 10-minute advantage enjoyed by Gwen. She never let Buddy forget either. She knew it bothered him, that his best friend was older. In a fun way. Both families actually met in the hospital, they never knew each other before. Both sets of parents lived just a few houses down but had no reason to interact until baby girl and baby boy were crying in the hospital nursery.

Every family in the small town of Lake Hill visited the Shakey lakes park every summer at some point, even the winter too if they enjoyed ice fishing. Bud and Gwen looked forward to these camping trips all school year. They planned out what they would do first when they got there. They passed notes in school about how they were going to make the best sandcastle. They couldn’t wait to get one of those “Bahama mama,” slushies from the shack. The shack was the little shop where you could get candy, fried food, and the famous slushies. The arcade was there too. So many hours playing pinball and Teenage Mutant Ninja Turtle stand up arcade. And then.. they just didn’t go anymore.

“When did we stop going here every summer?” Bud asked. “I feel like we were pretty young when we stopped going. Isn’t it weird our parents took us like 30 minutes into the city to roller-skate or see a movie?”

“Our rents never explained that either,” Gwen said. “The only thing I remember is something about the woods, maybe the state closed the park due to environmental concerns? I don’t know, felt like they didn’t have a good answer.”

As kids you roll with whatever your parents are doing. It’s like one day you’re packing up getting ready for the big weekend at the park, the next you never missed it and moved on to the next family adventure.

“I just recently thought about that,” Bud said. “I looked it up for the first time online and saw it has been open and never closed like out parents said.” Gwen shook her head with an approving nod. “Me too!” Guess we are still linked after all young Buddy Bud,” she jokingly blurted out. “Very funny, that’ll never get old.” Bud smiled, looking at Gwen with a slight sarcastic smile. “You might be older, but you’ll never be cooler.” With that, Bud pulled out a roll of quarters from his pocket. “Let’s find that arcade. And this time we can pour some vodka into those Bahama mamas.” Gwen giggled. “What?” Bud asked. She disappeared into the truck that towed the modestly sized RV his parents let them borrow. Coming out with her hands slyly behind her back, she slowly revealed an unmarked mason jar. “I got something stronger than vodka.”

Bud was delighted. “Wow, the Peterson’s famous moonshine.” “You are ready for this trip, eh?”

Bud didn’t know Gwen’s family that well. They were together a lot while they were kids, but usually it was Gwen coming over, or playing outside in the street until the lights came on. He didn’t know that Peterson wasn’t even her real last name. Gwen and Buddy took a pull from the moonshine. No chaser.

“Holy hell, that’s why stronger than I thought,” Bud said. “It’ll be better in those slushies big guy,” Gwen teased. “Let’s find the shack. I think I can see it from here.”

They locked the RV and turned on the garden lanterns. “Looks amazing,” Bud said. Not as amazing as Gwen, he thought. Quarters ready, moonshine secured in Gwen’s purse. They were headed to the shack.

“Just like when we were kids,” Gwen said. “Remember when you fell down this trail as we were running to be the first ones to get to the arcade?” “Ugh,” Bud moaned.

“Another thing you won’t let me live down.” “I was just excited to beat your ass in NBA Jam for the 1000th time. Charles Barkley and Dan Majerle always blew you away.”

“Whatever, I didn’t care about winning I just wanted to see those little guys fly down the court,” Gwen said.” “Sure,” Bud said. “You were so tired of hearing HE’S ON FIRE! As CMU alumni Majerle drained his 3rd three pointer in a row.” Central represent Bud joked, as he pointed towards the sky like David Ortiz after smashing a home run. “I know he went to Central, dummy,” Gwen laughed. “We saw him speak at the fieldhouse, remember.”

The two friends indeed attended the same college. Their freshman year they were inseparable. Going to college is stressful and psychologically rough for many young people. Having someone there you know is a blessing, and they took advantage of it. As the next few years developed, it saw the two young friends drift quietly apart. They still texted and met up here and there, but the closeness they felt had eroded.

The truth was Guy remembered every moment with her. He didn’t think she remembered that. It made him warm. “Of course, it was a blast!” “Seeing a real-life NBA player in the flesh was exciting, especially one that started out right where we were.” Bud changed subjects. College was great, but it reminded him of when their friendship changed.

“Let’s get those slushies, one more sip of that fire water before we do,” he said, looking at Gwen. She did a little shake, anticipating the hot ever clear that would fill her insides.

The shack was empty. Not closed, but it looked like it. All they saw were a handful of chip bags hung on the rack, a couple frier baskets that looked like they were last used 20 years ago, and some kid toys that molded a long time ago.

“Uh.. hello,” Buddy said. “Any chance we could get a couple slushies?” A weary older woman appeared out of nowhere behind the window that Bud and Gwen stood. Both took a studder step backward. “How can I help you two?” She said. Bud looked at his friend. “Um.. can we get.. what happened here? Are you open?”

“Sorry, yes of course, just wasn’t expecting anyone this early is all. You want some slushies, kids?” The lady looked odd, but nice. “Can we get Bahama mamas miss…” As Bud looked at Gwen, he wanted to leave, but before he could say anything, he turned back and saw two pristine looking dark slushies waiting in front of him. “Oh, yes.. thank you. What do I owe y..” “No charge”, she said. “Just happy to have people here.”

Not weird at all, they thought sarcastically. They both grabbed their drinks and cautiously walked to the beach. “Well.. cheers, right?” Gwen said holding back nervous laughter. They both busted out laughing and poured a generous amount of alcohol into their drinks.

“That was freaking strange,” Bud said. “Eh, like she said she’s just not used to serving this early, and probably not young people. This place looks like no one really uses the camp anymore. I only saw maybe 5 campers coming in.” Buddy didn’t remember seeing anyone else camping there. The weather was great, mid-June and this would be the first real nice weekend of the year.

The woods are alive. Buddy remembers his mom saying this to him. He was too young to understand. It’s too dangerous to go there anymore. Don’t you like going to the nicer arcade in the city? Look, I have an entire roll of quarters for you and your… friend to play with.

“Hello, earth to Bud.. you ready to play?” Buddy snapped back to reality. “Uh, yeah, let’s find those games,” he said, trying to feign excitement. They walked next door to the old arcade room. They found a door that was boarded up. Gwen went back to the shack window to ask where the games had moved. The window was shut, and no one was answering. “Where did she go?” she said, “she couldn’t have gone anywhere, we would have seen her.” Bud was starting to become paranoid.

Gwen is 14 years old. She is sitting at family dinner. Her mom and stepdad sit on either side, quietly enjoying a boring meal of chicken cordon blue. Gwen drops her fork.

“Why don’t we go to the park anymore?” Both parents stop, looking at each other. “What Park?” Gwen’s mom says. Gwen explodes, which she never did. “SHAKEY LAKE PARK, that’s fucking what!” Her stepdad calmly sends her to her room and Gwen storms off, slamming the door.

“Bud, it’s a small town. I don’t think there is too many kids here anymore. It won’t be exactly what we remember almost 15 years ago.”

“Where did Flash and Billy go,” Bud asked with dead eyes.

Gwen froze. “Who?”

Buddy took a long drink, not looking at her. “Billy and Flash, the twins we grew up with.” “I barely remember, but they disappeared when we were kids. Then we, somehow, never went back to this place.”

Gwen was confused, and the hooch-drenched slush wasn’t helping. She grabbed Bud’s arm without thinking about it. Bud looked at her hand, then up at her. She reflexively removed her hand, flustered and brushing her hair back out of habit. Bud was confused too. Why did he have such an affinity for her right now.

9-year-old Buddy. “Where did my friends go, they were cool, they were born together right?” Buddy’s dad sadly looked at him. He’d had enough of these last few years. Single father raising two kids. Rare for any time. “I won’t tell you again Bud. They moved.” Buddy seemed to be ok with this explanation, which he’s already heard.

“Our parents briefly mentioned that something was here, like maybe in the woods or something. Should we go out there, or..” Bud was cut off immediately. “Heck no sir!” Gwen was now back in full reality. “Hey, let’s go back to the RV, enjoy those lights. We can put some music on and talk about old times. Can we please enjoy this weekend?” Bud thought for a second, then happily complied. “Yeah, you’re right G. Time to relax.” Both were getting eaten up from the inside but decided to shrug it off and enjoy their time together. They were on vacation after all.

That night Buddy had a terrible vision of humans, or at least things that look like humans, walking around the Shakey lake campground. They traveled in packs of twos. They were dressed in regular clothes but had the smell of wild animals. Bud tried to wake Gwen up but saw nothing but an empty bed where he thought she was sleeping. He searched an endless cavern of darkness. The “things,” were surrounding his camper now. He panicked, screaming for Gwen. He ran full speed down the hallway, looking for..

“BUDDY, WHAT THE FUCK?!” Gwen dropped the breakfast supplies she was holding all over the ground. Broken eggs, raw bacon, and pancake mix was now feeding the earth. Buddy sat up, drenched in sweat. “What the, what the fu.. where am I, are you ok, what happened to you, where were you?!”

Gwen got Bud calmed down and convinced him to go to the showers not too far from their camper.

Buddy returned, clean and ready to be caffeinated. “I saw something last night. These.. “Land walkers,” were coming towards our RV. I don’t know why I call them that, it just sounds appropriate. They looked like humans, but.. not. They walked like humans, but I knew there was nothing inside. There were dozens, but they were all paired. Like a psycho gemini team. They saw me but wanted you. I tried to find you. You weren’t here. Where did you go?”

“Bud,” Gwen said, “You passed out.” “We must have gone asleep at about the same time. It was a dream dude, nothing more.” Bud took this in. “No..” Bud said but stopped. “Guess that Bahama mama kicked the crap out of me.” “Yeah you always were a Nancy when it came to drinkin,” Gwen said as she started cleaning up the dead breakfast of 2021. She looked up at Bud, locking eyes. She never noticed how similar his honey brown eyes were to his.

Gwen was playing outside her home as a little girl. Maybe 5. She saw her mom speaking to someone out of view. Gwen knew her dad wasn’t home, so it was strange that her mom would be talking with anyone while she was playing.

“You have to go.. you have to go NOW,” is all Gwen remembered. “I won’t let that happen. Not to our kids.”

Gwen cleaned up the vagrant egg and bacon remnants and made sure her best friend’s campsite was pristine. “I’ll make sure we still have a good breakfast, Bud. Sorry, I just got freaked.”

Buddy was happy to see his best friend alive, and still with him. “No, no problem at all G. I’ll help clean.”

Bud and Gwen enjoyed the second night with much less incident. The lights looked great, they took a dip into the lake, they made burgers and popcorn. It was a camper’s dream.

Gwen jumped from the vibrations in her back pocket. “Oh, my mom’s calling.” “That’s so crazy, I didn’t even remember I had this thing.” Bud agreed. “Me too. Glad to leave the thing alone for a while.”

“I’ll call her back. If it’s anything important she can text me.” The phone vibrated again. The contact’s name just said “MA.” “Sorry Bud, I’ll just text her real quick.” “Call her, its ok.” Bud said. “No, I don’t want to talk to anyone but you right now. If I talk to her, I’ll be taken out of my vacation vibe.” She made a kissy face at Bud and walked a few feet away to text her mom that she was ok, but please don’t bother me this weekend.

She discovered a text was already waiting for her. She put her password in to unlock the phone. “Get out of there,” is what her mom sent her. Followed by multiple messages explaining the same. “What the hell?”

Gwen called her mom. One full ring didn’t finish before she picked up. “Gwen, you have to leave. You can’t be around Buddy. I’m sorry I thought it would be ok. I thought they wouldn’t find you after we left that campground forever.” Gwen tried to slow her mom down. “Mom, MOM, what the hell are you taking about?” It was no use. She wouldn’t stop.. until the phone cut off.

Gwen, you have to leave. Leave the RV, leave your things. Just get into the car and leave as fast as you can. Gwen, they only take one. They only TAKE– Phone dies. Gwen tried to get her mom back. “Mom, mom… mom, what the.. hello? Where are you?”

Bud was enjoying himself by the fire, but his mood changed immediately when he saw the expression on Gwen’s face coming back to him. “Whoa, what happened, is everyone ok?” “My mom was hysterical, said we had to leave, something about taking someone, I don’t know I’m all turned around. Then the phone cut out.”

“What do you mean taking someone? Did she say anything else?” “I just told you exactly what happened, no she didn’t say anything else, she just hung up or got cut off.” “Sorry Bud, I don’t mean to yell but this is freaking me out.”

Bud and Gwen got quiet. Bud’s head was spinning. The air was getting cold. The woods were rustling. Bud couldn’t stop thinking about his nightmare. The land walkers. Walking two by two. They take.. they only take.. they take one. How did Bud know that? He didn’t hear Gwen’s mom say that.

Gwen, we have to go. NOW. Gwen hesitated, but Bud grabbed her arm so quick she barely had to time to think. Ow! Bud, what the hell has gotten in to – Bud shushed her.

“Gwen.. quiet. Something bad is happening. I don’t think we have time.” “What are you talking about? Bud, it was just a weird call from my mom, she must be having a long night and just misses me that’s all.”

Bud successfully got Gwen into the truck. He unhooked the RV and didn’t bother packing a thing up. He was ready to speed through a wildfire to get out of there. “Gwen, why did we stop coming here?” Bud asked. “You know why, our parents said it was something about the woods, the grounds not being safe, we never questioned it. For all we know there was a murder here or something and they just didn’t trust this place anymore.” Bud paused. “They lied. They all lied.” “I love you more than anything Gwenny.” Gwen blushed slightly. “I love you to Bud, I always have you know that.” “But I love you like a sister. I never had a sister. There are things out here Gwen. I’ve seen them in my dreams. They .. take people. Special types of people.

Gwen looked at him like he’d lost the last of his mind. “Bud, just get us out of here.” Bud looked at the ignition. “We were born on the same day. You don’t have a dad and I don’t have a mom. They knew they had to keep us away from this place. They were too late, even all those years ago. This place has been waiting for us. They only take one. They take one of the pair, to cause lifelong suffering.” They –

Bud’s driver side window shattered. Gwen screamed. She buried her head in her hands. Try hard as she could to muffle out the sound, she could still hear the gnawing and gnashing of bone, muscle, and sinew being shredded by hungry.. things. After the air was still, she looked. No trace of Bud. No trace of her long-lost, never known twin brother.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e8d2f57f-2832-421f-aac0-207ea9b5e3e5",
                    ThreadTypeId = 1,
                    Title = "Don’t Trust the Boy",
                    Content = @"
They didn’t believe me. Not the couple’s family after their house burned down. Not the daycare when half the kids mysteriously fell ill. And not the police when I attempted to end this plague with my bare hands. For perhaps the final time, I am begging you, all of you, please believe me when I say that god damn boy is evil.

Many years ago, I had lived next to a beautiful couple. The two were happily married, well-known in the community, and had big dreams of starting a family. When the day finally arrived to present their beautiful baby boy to the world, everyone was ecstatic.

For a time, I was one of those people giving them congratulations and proclaiming to treat that young boy as if he were part of my family. And for a time, I believed in the happiness associated with their new life.

But… As many people fail to do in the face of something so perfect, I realized I was neglecting to answer some fundamental questions. Namely, when exactly was this baby born? Surely there had to have been a date or a hospital where this child had come into the world. And yet, I couldn’t remember exactly when the baby arrived. As far as I could remember, there was no talk of the mom giving birth at the hospital or rumors of a large family gathering at his birth. No… I recalled us simply showing up to the young couple’s house one day where they had a baby in hand.

But, home births happen. Maybe they preferred it that way. Still, there was something else bothering me. For there to be a birth, there had to be a pregnancy. The more I thought about it, the more I realized I hadn’t seen the wife show any signs of pregnancy for the past 9 months. And neither of them had mentioned they had a child on the way. Adoption was an option, but the couple had adamantly said for years that they wanted a child that they gave birth to.

Yet, when I’d bring up these concerns to people around town, I was always met with looks of confusion. This, along with a strong belief that the young couple had indeed given birth, there was a pregnancy, and they remembered the announcement vividly. I’d counter with evidence that there were no pictures of the pregnant wife and the lack of official birth records online.

But it didn’t seem to matter. People were convinced. I mean, how could they not be? The baby was there. “He has his eyes and her ears.” They would say.

It was a rainy day in October. As I’m getting ready to leave for work, the baby stares at me from the window. I’m not sure how or why he was there, but the look in his eyes is one of intense focus. Admittedly, it gave me a profoundly eerie feeling. There was a deeper level of contemplation that shouldn’t have been there for a kid his age. He didn’t have the look of a child attempting to figure out why all these funny shapes were moving around him. No, he was far beyond that. I could see in his eyes that he was plotting.

I shook it off for the drive to work, but my confusion came on strong when I arrived. The day’s activities were a footnote to the unshakeable dark feeling I had carried with me since that morning. I couldn’t know what to expect, but my fear of something awful happening was realized when I got back.

What was once a lovely family home was ablaze. Years of happiness adding fuel to the roaring flames. The tears of the neighborhood were drowned out by the yells of first responders and sirens. A truly chaotic scene was unfolding before our very eyes. Despite it all, the hardest-hitting moment would come long after the fires were tamed. It came in the form of a press release from the local police. The young couple was dead. Their bodies were found in each other’s arms, charred beyond recognition. The cause of death was later determined to be asphyxiation from the smoke.

A miasma of sadness hung in the atmosphere for months. All of us were profoundly hurt by the tragedy and looking for answers. Why did this happen? How could this happen?

But, in my own sadness, I had again neglected the critical question. What happened to the baby? There was no third reported body found. No update in the news even referenced a child that was being looked for.

I tried asking around, even speaking to the family of the deceased, but I got the same response every single time, “What baby?” Everyone I spoke to insisted that the couple never had a child, which was supposedly a significant factor in why this was so sad. There was apparently nothing to continue their memory. A complete 180 from the narrative just weeks prior.

There was no way I just imagined this. There definitely was a child. People did know of his existence. And now, after the family passes, there’s nothing? I did some digging on social media and even searches for pictures of the couple with the child returned nothing.

Over time, I had convinced myself that there must’ve been a massive misunderstanding. However illogical my rationale, it was more comfortable to believe than the alternative.

Some months pass, and one of my close friends asked if I could take their son, Raz, to daycare because they had to be extra early for a meeting at work, and I was happy to help.

The morning I had Raz checked in, a woman bumps into me at the door as I’m leaving. Before I could say anything, she was already hurriedly asking where she can register her son, Gordon, and if it was too late.

I wanted to explain that I didn’t know much about the daycare and that I was just dropping off my friend’s kid, but I froze for a moment when I saw the young boy behind her. I had never seen this woman or her child before in my life, and yet… I recognized that kid. Or at least, he looked like someone I knew.

In a flash, it all came back to me. The young couple. The fire. Everything… That young boy had the face of that missing baby, only aged up a couple of years. Those dark calculating eyes penetrated deep into my psyche, and I could feel sweat start to form on my brow. At that moment, I recognized I legitimately feared this kid.

The woman gave me a bewildered stare as I stood silently contemplating all of this. With a simple, “Excuse me,” I moved past her out of the building. I sat in my car, in disbelief of another impossibility. If that was indeed the same child, he had aged at least three years in the span of a few months. For my own sanity, I landed on the notion that the two simply looked incredibly similar and that there wasn’t a real reason to stress about it. Still, deep down, I knew that wasn’t the case.

A couple weeks to the day went by without anything abnormal happening until I got a call from my friend. They were in tears, and eventually, as I got them to calm down, I came to understand why. Half the kids in the daycare had come down with a mysterious illness. The severity of this illness resulted in some… grim… results for a few of the children. Raz was one of those impacted. He would eventually come to make a recovery, but there was lasting damage.

As far as the daycare and medical personnel could trace it, it seemed that a kid who showed mild symptoms a couple of weeks prior was ground zero. I nearly dropped my phone at that news. I half-screamed at my friend about how some new kid named Gordon was registered that day and how they were likely the root, but they, understandably, didn’t care. Their attention was focused on Raz and asking about my own health.

After talking for a bit longer, I assured them I was okay, and as soon as the conversation was over, I got into my car and sped in the direction of the daycare. I nearly busted down the door as I sprinted towards the front desk and asked if they had a kid named Gordon register any time recently. There was some back and forth about who I was, so I lied and said I was a relative, but that didn’t get me anywhere. Apparently, no new kids had registered in the past month.

I argued that that was impossible because my “relative” had come in two weeks prior wanting to register her kid. But again, I was told that no one had done such a thing.

It had happened again. Somehow this child was gone from everyone’s memories except mine. I went to the local police and tried to share my findings, but I was basically laughed out of the station.

For another few months, all was quiet until I was driving home late one night. I stopped at a convenience store to grab some snacks, and as I’m figuring out what I want, a father walks in with his young son in hand. Instantly I get a sense of foreboding. Before I even laid eyes on the two, I had a strong feeling of who it was. When I peeked over the rows of snacks, my fears were confirmed. Also, as expected, the kid was just as aware of my presence as I was of his. I didn’t even bother to get what I wanted. I simply walked out and drove home.

The following day on the news, the top story was about a single man shot and killed just outside that same convenience store. Whoever did it didn’t even take anything. Just a stone-cold random murder.

In the following years, I witnessed the precursor to multiple tragedies around the world. Sickness, violence, random fatal accidents, emotional trauma, etc. The one connecting thread was that boy. Whether in person or on social media, I’d always see his damn face first.

He always remained a child, but his age varied wildly. Mostly he took the form of a baby, sometimes three or four, maybe even as old as ten. But undoubtedly, it was always him. I’d try and do my best to stop some of these events before they happened, but my efforts never mattered. There would always be a moment where something went horribly wrong.

One of the scariest things about it all is that no matter whether I saw him in person or on TV, he’d always be staring at me. I could see a video of a family walking the street with their kid, and I’d catch him looking directly at the camera. It was as if he knew that I was watching and wanted me to know that he was always watching me back. There was never a moment of peace.

My mental health degraded heavily over this period. I was in and out of therapy, picked up a bad smoking habit, and ruined more than my fair share of relationships over what some had deemed an “unhealthy and twisted obsession.” It had become unbearable.

One day after a dreadful night, I decided to sit at a park near my house to try and relax for the first time in a long while. For the first ten minutes or so, I was succeeding. The cool air on my face with the birds and squirrels living peacefully brought a sense of calm.

That calm, however, was quickly shattered when that familiar sense of foreboding hit me with the force of a semi-truck. Walking towards me was the boy, this time aged around six or seven, and a new couple. One that deeply reminded me of the first. Flashes of the feelings I had when I first saw that fire circled through my mind. Fear, anxiety, sadness… But something new accompanied it. Absolute pure rage.

I was livid that this boy was somehow responsible for so much damage. All of the lives he had ruined. And he didn’t even dare to face the horrible things he’s done. He just destroys and disappears. Hoping that those damaged won’t remember he was even there. But I did. And if I was the only person that was going to remember, then maybe I was the only person who could do something about it.

I flicked my still lit cigarette onto the ground, got up from the bench, and began to walk towards the trio. My heart beat through my chest with every step. A million thoughts raced through my mind, and before I knew it, I was slowly raising up both hands and contorting them to perfectly fit around the boy’s neck. Finally, when I was just within reach, I sprung forward and let out a visceral scream. But the only thing I came into contact with midair was a pair of knuckles cracking me against the temple.

I blacked out for a moment and woke up to a man sitting on top of me. He was raining down a hailstorm of punches with a woman screaming behind him. My world went dark again, only to come back to a group of people pulling the man off of me with another holding me down in place. Watching all this chaos unfold was the young boy who had a massive smile plastered on his face. Those soulless eyes lapping up every bit of my pain.

The cops arrived shortly after that. I was charged and subsequently spent time in jail. In the police interview, I begged them to understand why what I had attempted was necessary. I laid out my case, explained the connections between multiple occurrences, and warned them that the family was in danger. The response was as expected, disbelief. And for their hubris, that couple died in a car accident only a few days later. And even then, was I believed? No.

Interestingly enough, the charges were later dropped due to “lack of evidence.” Witnesses had claimed they saw me walking out of the park when a man randomly attacked me as I tripped into him.

And just like that… So easily… Life had gone back to normal. It wasn’t until I was back sitting alone at home that a new realization hit me. I couldn’t stop this. It did me no good to even attempt to pretend like I was in control here. There’s a dark force beyond my control, and I… We… Are mere toys to be moved around at its whim. It’s not something I’m okay with, far from it. But at this point, what can I do?

All I can try to do is live a good life regardless of what I know is coming, and for a solid time, I did that to the best of my ability. This coincided with a lack of seeing that boy in any form. For a time, I thought I could actually say that I was happy. Much in my life had gone right, and then like a ghost from the past, he walked into my life again. This time in a form I had never seen previously.

He was grown. Or at least as much as I had seen him grow. He was perhaps in his late teens, but those dark, hateful eyes remained and were the unique identifiers aside from the remnants of a familiar face. I only saw him briefly in a crowd of people, but his presence was undeniable, much like the knot growing in my stomach was. He appeared as though he had been watching me for a while. Waiting for me to make eye contact. And as soon as I did, he left.

To this point, nothing bad that I know of has happened. Though, I get the feeling that something awful is coming. Maybe something that goes beyond a small family or one person. Perhaps something that involves all of us. I don’t know… And not knowing scares me.

So I write this as a direct warning. If in your life you see a child you feel is off… Or if any of you notices a kid’s placement in someone’s world doesn’t quite fit with what makes sense… Maybe you even sense an abnormal level of hate emanating from what should be an innocent kid… Then remember these words… Don’t trust the boy.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "ffa37ac4-575a-4b06-b4b9-23365c62d15c",
                    ThreadTypeId = 1,
                    Title = "The Malice of Mary Marble",
                    Content = @"
Have you ever known one of those people who made you feel truly unsettled? For me, it was Mary Marble. She was a pretty girl, all things considered. She had long, straight jet black hair, a fair complexion and spoke in a cute, squeaky voice. She was neither bigger nor smaller than any of the other kids at Grady junior high, but she was always quiet and somber. Some would call her simple or slow as she seemed very withdrawn and never went out of her way to talk to anyone, but her grades were above average and she was always quick to correctly answer any question the teacher would send her way.

By all accounts, there was nothing off putting about the girl, at first, but there was something in her eyes that would always send a shiver down my spine. At times, while kids loudly yelled and stomped through the halls of the school, Mary would just lean with her back against the wall and stare. A lot of times she would just gaze off into space, seemingly lost in her thoughts, but other times she would lock eyes with one of us fellow students. There was something unnerving about the way her dark eyes seemed to cut into us so deep that we couldn’t look away. It was as though she had us in a trance and was somehow feeding on our subconscious minds.

That’s what we’d tell each other anyway, in a much more crude and childlike fashion. I’m not sure if I bought into such silly theories, but it was a defense mechanism of the insecure teenager to go out of our way to point a finger at someone else before one could be turned to us. Regardless of what the reasons were, her stare would cause me to feel incredibly uncomfortable to the point I would even become queasy. I wasn’t sure what it was, but something was off with that girl.

I remember when she first appeared in our home room on the especially chilly morning in October. I can’t remember the actual date, but I think it was around the middle of the month, if memory serves. I was only eleven years old and was still quite shyly getting used to the sixth grade in an American school. I was born in a little town in Scotland named Clackmannan, but had been forced to move to the states after my father received a job offer he couldn’t refuse. I was ten when we entered the country and the school year was at its midway point, so I had a lot of trouble fitting in at first. It didn’t take long for the novelty of a foreign kid earned me a handful of friends and fortunately they would be attending the same junior high as I in the fall.

I was short and skinny and quite the nerd, but my friends were of similarly dorky tastes so it worked out. Mary was escorted into the class by the school counselor. He was a dodgy and short, balding man whose hair wrapped around the back and sides of his head like a pair of earphones that had slipped off the top. He had a thick caterpillar mustache that fluttered from his breath as he introduced her to the class. He seemed to be abnormally stuttering as he pronounced the girls name to us and he quickly turned around and darted out the door as soon as the information was relayed.

Mary stood in front of the room wearing a black ruffled dress that came down to her black and white striped knee high socks. She wore a similarly striped long sleeved shirt under her dress and her long dark hair flowed over her shoulders like a stream of black ink. I had just started really appreciating looking at girls and I was instantly struck by how pretty she looked. She didn’t seem uncomfortable or awkward as she stood before us in silence, but I stared into her face and I noticed that she had not blinked since I began looking at her. I quickly averted my gaze when she turned her eyes to meet mine and I suddenly felt sweat form on my brow.

I glanced back up for a moment and found her still glaring at me, wide eyed and expressionless. I had fully intended to just take a quick peek before turning my gaze back down to my closed notebook, but I couldn’t look away. As I started to feel a chill across my spine, the teacher broke the silence in the room and asked Mary to take the open seat at the back of the room. Every head in the room followed the little girl as she paced gingerly to the rear of the classroom and they darted back forward when she turned to sit down. It became quickly clear that the new girl would be the primary topic for the gossiping children of Grady Junior High for the next few days.

I was a shy kid in general, but almost paralyzingly so when attempting to talk to girls. “H-hi,” I said, nervously, to the wide eyed girl leaning up against the lockers after a sufficient amount of dares from my friends forced me to do so. She just stared at me as though I had attacked her with a slew of insults rather than just offer her a simple greeting. Feeling my face flush I continued, “I-I’m Jacob, um, everyone just calls me Jake though,” I giggled feeling more awkward than jovial. Her dark eyes still just glared at me and I was feeling incredibly uncomfortable as my friends chuckled from somewhere behind me.

“O-ok then,” I stuttered and turned to walk away. “Do you wanna know when you’re gonna die?” The little girl asked in her squeaky yet monotone voice. I turned back to face her, genuinely freaked out by her question. “Huh?” I replied. “I can show you,” she continued, “if you want.” I was stunned and wanted nothing more than to get as far away from her as I could. As I looked at her, the pale face that had previously seemed quite lovely to me now looked more sinister and malicious. There was something behind her eyes that sought to drill into my soul and I felt a sudden panic grow from within me. I shook my head aggressively and said, “N-no,” my eyes were fixed with hers as my head waved from side to side, “No thank you.”

As soon as my words left my lips, I turned on the spot and walked quickly away. I got back to my friends and told them what had happened, to which they laughed and jeered and started making fun of her from a distance. Word spread quickly about the child’s strange ways. Even more so after a variety of other kids attempted to socialize with her. Regardless of the mockery and her complete lack of anything resembling friends, Mary never seemed brought down by things. Well, not that we could tell anyway. She seemed neither troubled nor content, really. Just neutral and blank.

During the summer before our seventh grade began, Cindy Baxter, who I had also shared class with the previous year, was killed in a horrific accident. She had been riding her bicycle down the sidewalk only yards from her home where she lived with her younger sister and mother. Her father, according to rumor, had been killed while serving overseas in the military. I only knew the girl in passing, and wasn’t familiar with her family at all, so take all of that with a grain of salt. There was a long, ironwork fence that ran the length of the section of sidewalk she was coasting her bike along, so when the small sedan that was speeding down the road that paralleled the path unexpectedly blew a tire, she did not have the slightest chance to get out of the way.

As witnesses would report, the car instantly skidded to the side of the road causing it to careen into the strong, metal railing, with little Cindy Baxter caught between the two. By the time the story made it through all the exaggerated channels of the preteen gossip circuit, it told that the poor girl had been severed in two by the impact. By the time we returned to school in the fall, the story had grown far more morbid and grotesque, but the undeniable point remained. The poor little girl had died an untimely death, and that tragedy would only be the first of many to come over the following year.

The room fell silent with the exception of a few kids crying when our home room teacher made the announcement about Cindy’s death. We had all heard the news already, as it had happened some weeks before school began again, but the pain was still fresh to a lot of the kids who had known her. This, of course, set off the gossip chain once more, and the stories would be rehashed in vivid detail through the rest of the week. Tasha Holmes, one of Cindy’s best friends, mentioned that Cindy had made the effort to speak to Mary Marble several times in the days before school let out for the summer.

She told us that the wide eyed girl had apparently asked Cindy the same question she presented to me in our first and only meeting. When Tasha asked her friend if she had accepted Mary’s offer of prediction, Cindy just looked off to one side and shook her head. She had been distant and dreamy over the days that followed, according to Tasha. Johnny Roma, a quick witted and outspoken member of my little group became convinced that Mary was a witch and swore he would confront her about the whole thing. The rest of us laughed at him and told him he should stop staying up late to watch horror movies with his older brother. He mumbled a slew of words that I would never be allowed to say, without fear of the back of my father’s hand, and stormed off down the crowded hall.

As the days progressed, more kids had started to hold Mary responsible for little Cindy’s death. They were convinced she was anything from a murderous little psychopath to the antichrist herself. I found the whole thing to be blown out of proportion. The accident was tragic and sad, but nothing that would appear to have been supernaturally encouraged. Tony and Charles, the other two members of my dorky friends circle, shared my mindset on the topic and were tired of hearing about it. Even at our ages, we knew that dwelling on something such as this would not allow anyone to move on from it.

Charles did feel a little bad after he barked at Johnny for spilling his silly theories one day. We had just arrived at lunch and Johnny was just staring at Mary from across the room while going on and on about what a monster she was. Charles just lost it. I can’t recall how the dialogue between them went, but it ended with many eyes in the room transfixed on the two of them until Johnny yelled at everyone to leave him alone. His words were more colorful than mine, but you get the idea.

Later that same day, I saw Johnny talking to Mary from a distance. His demeanor was confrontational at first, but gave way to something very different as the conversation went on. I noticed his shoulders droop as his head hung low, while she still stared in her normal blank expression. My friend turned on his heels and ran up the hallway while she watched his exit. He was out of my sight before he escaped hers, but as I turned to look at her, she turned again to face me. We locked eyes as other kids walked back and forth between us. Even when our line of vision was blocked, I could still feel her gaze.

It felt as though we were the only two people in a deserted room and I felt cold and lost the longer we stared at each other. We finally broke eye contact when a bigger kid charged down the hall, knocking me almost to the floor. He seemed on a mission of sorts and paid no attention to my stumbling and grasping for anything to prevent my fall. I finally composed myself and looked across the hall to see that Mary no longer stood there. Though the experience had been less than comfortable, I went on about my day as any other, though I would never see Johnny again.

The news of his death greeted me early the following morning after my mother received a phone call that caused her to gasp and place a hand over her heart. Johnny had been walking along a nearby train track late the previous night, apparently very distracted as he did not notice the sounds of the approaching train. There was little left of his body, which made his identification no easy task, but word leaked out quickly once the information was discovered and had reached across the little town of Grady before we could lay foot on school grounds. My mother had given me the option of staying out of school for a few days, as Johnny and I had grown close over the last year, but even at my young age, I knew staying at home would only lead to brooding about the loss of my friend.

Children and teachers alike shed tears for Johnny when school began that day, and many would leave to make their way to their respective homes over the following hour or two. Charles, Tony and I sat in silence at lunch as we could not seem to find the words to break the suffocating quiet. Other kids still cried at nearby tables, while others hung their heads and attempted to eat their food. Johnny had been a fairly popular kid, which had always puzzled me, along with the others of our dorky little group, to why he would spend his time with us. He said he could be himself around us, but always felt like he had to put on a show with others. Though he enjoyed his popularity, he would often remark on his envy for the rest of us and how we could blend into the background and go unnoticed. It had seemed insulting at first, but his sincerity could not be denied.

Over the course of the school year, another three children would meet their end, each in a more gruesome fashion than the last. Bridget Morgan, an eighth grader who had always been athletic and lively, dropped to her death from a nearby cliff surrounding a quarry. She had struck sharp rocks and pointed tree roots during her descent which had torn her poor body to shreds by the time her fall came to it’s grisly end. Billy Jordan, a sixth year student was torn apart by wildlife in the woods that surrounded our town, presumably bears or wolves. Finally Andrew Barnes, an eleventh grader and friend to many, had fallen into the wood chipper on his grandfather’s farm. Many believed that a curse had befallen the town of Grady, and my little group of friends had grown convinced that the curse was named Mary Marble.

By the time our seventh grade came to a close, we had added three new friends to our growing club. Sara and Veronica Burgess were twin sisters who shared the same grade as us. They were both really pretty with matching long, red hair. Sara was more freckled than her sister who was far more outgoing than her, but they were equally as sassy. I had developed something of a crush on Sara, though I would not freely admit that to anyone until some years later. They were both a little nerdy, like the rest of us, but it only served to make Sara more appealing to me, with her thick rimmed glasses and slight lisp when she spoke.

Josh Holden, who would suffer the wrath of many a ‘Holden me groin’ joke over the years, was a quiet kid, but could be quick with a joke once you got him talking. He had shaggy, brown hair that was parted in the center and sported a fairly stocky build for his age. They filled out our group quite well, and they shared our belief that Marble was the root of all of the evil that had befallen our school. We made plans to find out everything we could about her before school resumed the following year, but we could not have predicted where our investigation would take us.

It was rare for anyone to see Mary outside of school, and nobody had the slightest clue where she lived. She had no friends to speak of, nor did she seem intent on making any. The fact that many kids had become terrified at the thought of opening up a conversation with her, had given her quite the status as the school’s biggest outcast. Every now and then, I would feel bad for her sitting alone at a corner table in the lunchroom, but then I would recall the discomfort I felt whenever our eyes would meet. It was something of a conundrum, since I knew all too well the feeling of being rejected because of my differences. Even so, she was a pariah, and I knew behind that cute little face lay pure evil.

So, my motley crew would spend the weeks after school let out investigating the town’s history for any circumstances in it’s past that resembled our current situation, or pacing the town’s streets seeking any evidence of Mary, herself. Maybe we had seen too many movies or read too many stories of recurring atrocities in the smaller cities of the nation, or perhaps we just had no clue where to look for answers to the events that had brought this town to a seven O’clock curfew. Sure, older teenagers would still party into the wee hours of the night and groups of kids could still be seen spending time in the parks and playgrounds after the sun went down, but the police only seemed primarily focused on children who were alone after hours.

As it turned out, Grady did harbor some mystery in its history, which Sara and I agreed sounded like an excellent title to a Scooby Doo episode. She had the cutest giggle and the way she would cover her mouth with the tip of her fingers when she laughed would make me smile like an idiot every time. Charles would often nudge Tony and they would both silently chuckle when I would stare at Sara with my cheesy grin, but I would just flip them off when nobody was looking which would only serve to make them laugh harder. Though our research gave us some grim history, it did not provide us with any answers to the town’s current dilemma.

It seems there had been a woman, some hundred or so years before, named Daphne Clayton, who had committed a rash of brutal homicides of the children of Grady. When she was finally captured and interrogated on the motivations behind her horrible crimes, she confessed that the death of her own children had driven her to such deplorable acts. Regardless of her claims, there was no evidence that she had ever been a mother, to which she would insist it was many years before she found herself in this town. She was hanged for her crimes after a very short trial. It came out after some years of concealment of the true depravity of her actions, that she had, in fact, fed on the children she had murdered.

I lay the half eaten Twinkie, I had been chewing on, to rest when Josh narrated this particular except from the ancient newspaper reel he had jotted down during his trip to the library. Veronica looked as though she were close to spewing her own snack across the floor of our shoddy treehouse, while her sister slapped her on the back to help control her spluttering cough. Aside from the gagging Roni, as we had nicknamed her, nobody else was saying a word.

Though we had plenty of outrageous theories about Mary Marble being the reincarnation of the cannibalistic Daphne Clayton, we had nothing rational or realistic to go on. We had begun to lose hope in our short career as budding sleuths until we heard the news that a child had gone missing. Gregory Banks and his family had only recently moved into town, and he had not yet attended school in the area. Nobody knew much about them, but they had apparently migrated here from the big city to live in a less crime ridden environment. Bad idea, Banks family.

A search party was set up quickly, though it bore no results and the mystery of the missing Banks child would be just another log on the fire for us. We couldn’t give up on our quest for answers, even though the police department seemed as dumbstruck by everything as the rest of us. Accidents can be chalked up to chance, but a missing child could mean so much more.

As August was approaching, we had almost given up hope in finding answers before a new school year began. Though we had grown close, we weren’t spending as much time in a group of late since our investigation had become an obsession that had started to wear on us as a whole. We would still gather together a few days a week, but we had started spending more time apart than we had at the beginning of the summer. Not to mention, our parents had grown more restless since the Banks boy vanished, and were wary about letting us leave our respective homes. It was a rainy Tuesday, only two weeks before the beginning of the school year, when I received a frantic phone call from Tony.

He claimed he had found where Mary Marble lived. The reception was terrible on the glossy brown, rotary house phone my parents had owned since we moved into this country, so I could only make out so much of what he was saying. I asked him to repeat what he said multiple times, but that only served to frustrate his already frenzied state. He was in the middle of hysterically attempting to convey the information when he suddenly fell silent. “Tony?” I barked into the phone. “I gotta go.” He replied in a very subdued voice. “Tony? Are you ok?” I asked. The line went dead and I assumed he had hung up the phone.

His body was found the next day at the treeline of the same woods in which Billy Jordan was found. Tony was found with both legs and his left arm missing along with a giant hole in his chest where his heart used to reside. In reports I would not have access to until many years later, his limbs had appeared to be pulled from their sockets. That part still leaves a threatening lump in my throat to this day. For a few weeks, the woods were closed to the public while the police investigated the area since two children had brutally lost their lives behind those trees. After finding nothing they considered dangerous, it was once more opened back up to the public, though nobody seemed remotely interested in spending time there.

By the time our eighth grade began, many families had moved away from our little slice of hell on earth. Sara and Roni had been moved to Florida with their parents, but Sara vowed to keep in touch. We would write back and forth and occasionally talk on the phone, but eventually lost touch as many long distance friendships do. Charles and Josh were still around, but Tony’s death had taken a toll on Charles as they had been tight since kindergarten. He wouldn’t talk much anymore and remained distant no matter how hard Josh and I tried to break him free from the depression which consumed him.

It looked as though it was down to Josh and I to carry on alone after another month passed to find Charles and his family heading for another state. I wished him well and asked him to keep in touch, but he never did. I would find him again some years later when social networking allowed easy access to relationships gone by, but he had long since moved on from the events that shattered his youth, and I chose to leave it at that. Perhaps my face reminded him of memories that still haunted his dreams. Maybe he just had no care for friends of old. Regardless, he seemed to have a happy life now, and I wished him the best.

Josh and I had determined that we would follow Mary home from school the day after Charles left. Our first few days of attempting pursuit of the wide eyed little girl proved fruitless. We made the mistake of following too closely the first day which caused her to run after she saw us. We tried to catch up to her, but she was so much faster than us. By the time we buckled and held our splitting sides, she was far from view. We gave it a couple of days before trying again as we felt we had blown out cover, so to speak. As we neared the weekend, we tried again, but she easily slipped away from us since we followed from way too far behind, seemingly overcompensating after being spotted the first time.

We realized we would have to practice far more stealth to be able to track her to where she lived. On the Wednesday of the following week we ducked out of school early to make our way to the farthest point we had managed to follow her to. We ducked behind the trees until she came strolling by, swinging her bag back and forth while she walked. She appeared so cute and harmless in the way she carried herself, but we just knew something sinister lay below the surface. Once we were sure she had not seen us, we began our pursuit again from a safe distance.

We were doing our best to keep our footfalls silent, and would duck out of sight when she would, on occasion, stop and turn around. We lost her again after stalking her for a solid twenty minutes or so, but we had earned a new starting location for our hunt. We commenced our quest again the next day after ditching school at an even more premature hour than the last, only to lose her once more at the tree line of the woods on the edge of town. These were the same woods that Billy Jordan and Tony had found themselves torn apart in, though we had never had reason to believe that any hostile wildlife resided inside, especially after the sheriff saw fit to strip the caution tape from the trees. Even with that in mind, the knowledge that we lost Mary at the edge of the very same woods in which two of her victims had met their end, we thought it best if we armed ourselves before entering.

The weekend arrived and we would likely not have an opportunity to track our prey again until the next school day, so we chose to work on gathering supplies for a final showdown, should it occur. I can’t say I wasn’t nervous or apprehensive at the thought. I was fucking terrified, to be completely honest, but I felt a lot safer with Josh by my side. He was way stronger than me, but I had never been much of the athletic type. He was already bench pressing a hundred and eighty pounds in our weight training class. I managed to push a hundred and fifteen one day, but that was with him spotting me and doing most of the work. He would insist that it was ‘all me’ but I knew he was just being kind.

We had grown to be close friends since the rest of our group had separated, and I knew he would have my back if things got out of hand when we crossed into those woods. We spent that Saturday gathering together what we could. I managed to snatch the axe my dad used to cut wood, though it was way too heavy to swing, a baseball bat from when I had attempted (pitifully) to learn American sports and a couple of torches. Josh laughed and corrected my terminology to ‘flashlights’ when I presented them to him.

He had gathered two machetes, one of them pretty rusty but still sharp, and some football pads for protection for both of us. He also had some knee and elbow pads along with some really cool fingerless leather gloves that he swiped from his older brother’s room. He only had the one pair of those, and I can’t say I wasn’t a little jealous. He looked really cool suited up, but I felt a little awkward when I slipped the pads on as they felt huge on me. He said it didn’t matter how they looked, only that they kept me safe. He was right, but I still felt a bit silly. I suggested we try to locate some sort of protection for our heads too, and Josh slapped his hand to his face as he hadn’t even thought of that. We chose to abandon the axe when Josh agreed that it felt clumsy and awkward to swing, so I would return that to my dad’s shed later that day. We still had Sunday to make final preparations for Monday, so we packed up our ‘battle gear’ in the treehouse, and tried to spend the rest of the day having fun.

Sunday did not go as planned thanks to a horrendous storm that destroyed any plans for an excursion outdoors, so Josh suggested that we just lay out of school entirely on Monday to be able to be ready and waiting in the woods when Mary arrived on her journey home. It seemed a logical suggestion and I just spent the day relaxing in my bedroom while MTV provided music videos to serve as my soundtrack for the rainy Sunday. Monday came and I left for school, as far as my parents knew. I boarded the bus like any other week day, but darted off of school grounds as soon as the bus rode off.

I met Josh at the treehouse, where I found him waiting with two football helmets. Just like the, now soaking wet, football pads, it was too big for my head, but my friend had already thought of that and handed me a knit cap to wear under it. Not only did it provide enough padding to stop the headgear from rattling around on my head, but it made it a great deal more comfortable. We hung the pads on one of the tree branches to the side of our little club house and hoped they would dry off at least somewhat before we would make our way to the woods. After visiting a nearby convenience store to stock up on snacks, that would be our lunch for the day, using our mutually saved up allowance, we spend the rest of the morning and early afternoon waiting in our treehouse, snacking and talking, until we would make our way to the forest. It was a good walk to get there, and we were slightly concerned that passing drivers would alert our parents to our playing hooky from school, but those were inherent problems for rule breaking kids of small towns. Worst case is we would get a good chewing out from our folks if we survived whatever may unfold behind the treeline. It wouldn’t be the worst possible scenario to come out of this.

At around one O’clock, we loaded up our bags and began our stroll to the woods at the edge of the town. We had considered riding our bikes with our bags of supplies strapped to our backs, but they would be hard to hide from the approaching Mary Marble. Josh suggested that we at least take them halfway and stash them somewhere, to which I replied that I was worried they would get stolen, or even worse, that our parents would notice they were gone and know we had skipped school. So, we walked and tried our best to hide when cars would approach from either direction.

We weren’t the most effective with our attempts at stealthy maneuvering, and inadvertently drew more attention to ourselves with our leaps behind fences and trees while loaded up with bulging backpacks. In the long run, nobody seemed to care what we were up to while they were consumed with their own day’s activities. After a little under an hour and a half worth of exhausting travelling, we finally arrived at the treeline. We hesitantly crossed through the trees to find the woods not as dense as we expected. This would make it hard for us to keep our presence hidden from our target, not to mention that we had not taken into account how uneasy it is to remain silent in the woods, especially in the fall and winter as dried and dead leaves lined the forest floor. We decided to climb into the trees and attempt to remain hidden in the leaves above. It may not be an effective plan when it came to following behind our mark, but we’d jump off that bridge when we got to it.

My feet had already gone to sleep by the time Mary passed through from where the outside world met the forest. She hesitated for a moment and looked from side to side. Luckily for us, she did not look up, as the trees weren’t nearly as full as they were in the summer and we would likely be as obvious as a bright red and throbbing thumb sprouting from the tree’s bark were she to glance our way. We were able to watch her as she strolled forward in as straight a line as she could traverse over the bumpy terrain. Josh had the idea to toss a pinecone off into the distance to hopefully mask our descent from our trees. This only served to cause our mark to quicken her pace, though it also inspired the scurrying of a variety of the woods inhabitants. I could see a deer sprinting far ahead as well as some squirrels that had apparently been spooked by the assaulting pinecone. This would help camouflage the crunching leaves beneath our feet, which was a much more aggravated sound due to the pins and needles that shot through my legs as blood flowed freely once more.

As if brought to life by the combination of footsteps provided by Marble and ourselves, the forest had come to life with a cacophony of woodland creatures bounding between, up and across the trees. It was unsettling how silent and still it had been before the wide eyed girl entered, but it did allow us to remain unnoticed, or so we hoped. We would regularly hide behind trees in hopes of remaining out of sight should she glance to her rear, but all was working in our favor for the time being.

I was struck, suddenly, with the weight of how deep these woods appeared to run. We had been following from a distance for around twenty minutes, according to the digital watch on my wrist. Though it should not have been late enough for the sun to set, the world around us began to fall into darkness. My feet had been hurting for a few minutes now and I was starting to regret this whole thing. Josh looked equally as tired as I and our ability to remain stealthy had worn thin. We looked at each other while Mary walked on far ahead of us. Josh shook his head and shrugged his shoulders. I nodded in agreement. As exhausted as we were at this point, we would be hard pressed to put up much of a fight, should one occur.

I turned my attention back to the woods ahead and felt like a complete fool when I could no longer see any sign of Mary. Josh had just noticed, too, and he looked back at me with panic and frustration in his eyes. “Goddamnit!’ my friend swore in a little more than a whisper. We jogged ahead to the last point we had seen our target. It had become as dark as if it was the middle of the night, but we were uncertain about using our torches. We didn’t know if Mary had noticed us and was only hiding somewhere now, or if we had simply missed her making a turn. The last thing we wanted was to alert her to our presence if she was not already aware of it. That being said, we could not see anything. Josh pulled out his flashlight and turned it on while keeping his off hand shielding the light from spreading too far.

To add to our dilemma, we had been too busy paying attention to our pursuit, we had no idea how to get back out of these woods. Sure, it felt like we were going straight, but the terrain was uneven and bumpy and we were being guided by someone who appears to know this forest well. It had been a solid ten minutes since we had last seen Mary and we were basically running around in no particular direction now. This only served to make us even more lost as we no longer knew which direction we had come from. I was beginning to panic when I heard a sudden scream coming from where Josh had been standing. I looked around in the direction of the panicked yell to find my friend gone.

I ran to the last spot I had seen him, but saw no trace of where he had disappeared to. I was darting my eyes from side to side, looking for any trace of Josh. My heart was racing and I had begun to hyperventilate. I just knew she had gotten to him. Maybe she had been aware of us following behind her this whole time. Perhaps she was just waiting for the right moment to pounce. I started calling out for my friend, still attempting to keep my voice low just in case she had not already spotted us. It was at this moment I realized how quiet everything had grown. No birds chirping or animals running between the trees or up them. The dead leaves in the ground didn’t even seem to split and crack the way they had only minutes before.

My hands shook violently as I reached into my backpack to retrieve my torch. I clicked it on and shone the light around me taking no care to who might spot the glare through the darkness. I felt my bladder attempting to give way, but I fought against it. I was starting to feel quite sure that I would never see outside these woods again. “Jake!” I heard my name called out from somewhere far away. It didn’t sound as though it was coming from ahead or behind, but below.

I directed the light from my torch to the ground beneath my feet. I tried to follow the sound, to seek out where it was coming from. I felt my feet slip out from under me and I fell to the ground. I gathered myself together and pointed the beam of my light to the ground ahead of me. There was a deep ditch that sloped down to meet a hole In the forest floor. It was then that I noticed how wet the ground was here. That would seem to be why the leaves no longer crunched under my footsteps. Likely the reason I almost slid feet first into the hole, also. I gently crawled to where the circle of apparent nothingness lay in the middle of the woods and pointed my flashlight through the opening.

I could hear Josh calling my name from somewhere below and felt a huge weight lift itself from me when I saw him through the torchlight. He looked to be around twenty to thirty feet below, and I couldn’t help but wonder how he had survived the fall that found him there. He waved and pointed his finger towards the opening, which seemed unnecessary since I was well aware of what I was looking through. I finally noticed what he was getting at when I saw the vine-like rope that disguised itself between the wet leaves that lined the circumference of the gaping maw in the ground. I nodded to Josh and reached for the rope. I was nervous about scaling such a distance as the weight of the bag of supplies I carried on my back would surely surpass the capacity of my grip strength and muscles, but I had to get down there. The mouth on the forest floor was more than wide enough for me to fit through with my backpack in place, but my heartbeat sped again when I found myself hanging above the wide open space below. My descent was slow, but I felt a head rush of delight when my feet felt ground beneath them once more. My friend slapped me on the back when I landed and I noticed his palms were a fiery red after he winced from the enthusiastic greeting.

Josh explained to me how he had slipped through the hole, just as I almost had. He found himself falling through the air to face a certain death when he managed to wrap his hands around the hanging rope. The threads tore through his skin as he had a lot of momentum built up from his fall, but he managed to slow himself enough that when he landed, he remained in one piece. His palms were inflamed and the skin was shredded in places, so we wrapped them up as best we could with some loose fabric from his baggy and torn shirt. He joked about how he didn’t want to look like he was wearing a midriff if we got out of here, so we clumsily used one of the machetes to cut the cloth from his sleeves. Josh had very muscled arms for his age, and was not against the way his shirt appeared now, claiming he looked ‘buff’. We chuckled a bit, though we were still reeling from this whole experience.

Finally taking the opportunity to look around our newfound surroundings, we were both shocked to look upon the wide open space that lay before us. It appeared to go on for quite some distance, both in front and behind us. We could see walls formed from rocks and dirt on either side of us, but the cavern was easily some sixty or seventy feet wide, though we were no experts on measurements of distance. Being still confused about which direction we had come from, we were unsure about which way to go. Given the fact that Mary had vanished before us in a matter of seconds in a similar fashion to how my friend had disappeared, we had to assume that her location was also down here somewhere. Josh suggested that we follow the path we were facing, to which I shrugged my approval. His guess was as good as mine, afterall.

This cavern we found ourselves in was not as dark as the forest above us had become, so we felt no need to shine our torchlight ahead of us. We couldn’t tell where the light generated from, but it was as easy to see as it would be on a cloudless night with the full moon glowing from above. I’m not entirely sure why we didn’t question our situation a lot more than we did. Perhaps it’s because we were still children and still believed in the fantasies that our books and movies revealed to us. Maybe it was just that we were already too scared to seek rationality. Of course, it could be that we were just incredibly stupid for letting things go this far.

We followed the cave for maybe ten minutes before we reached a dead end. The path had become narrow as the walls on either side slowly closed themselves in front of us. Josh sighed heavily and we shrugged to each other before we turned around and headed back the way we came. Our pace had slowed dramatically, so it would be closer to twenty minutes before we strolled past the rope that dangled from the forest floor above.

We were growing increasingly tired from the amount of walking we had already done today and I was silently regretting skipping school in favor of this ridiculous quest. Another thirty minutes or so later, we finally reached what surely had to be our destination, though it thoroughly perplexed us to look upon what was before us. Off in the distance ahead, where the ground lowered to grow far deeper below the woods we had traversed what felt like hours ago, stood a quaint log cabin.

It was a very nice looking house, which we appreciated more the closer we got. It had green vines that appeared to weave around the cabin like a protective cage. The structure looked like it would be more fitting on the ledge of a mountain somewhere, overlooking the world below. It was the last thing we expected to see buried deep under the ground, but it strangely made things feel less threatening. Perhaps we were wrong about Mary. Maybe she was just a normal kid, who never blinked and lived in a cottage miles into and under the forest. Yeah. Totally normal.

We approached the building and took note of the warm light glowing from behind one of the second story windows. The curtains were drawn and we could make out no signs of life behind them. I glanced at my watch for the first time, since we reached the edge of town where the treeline stood, to see that it was close to seven O’clock. I knew that my parents would be panicking by now, especially considering the ongoing curse that had befallen our little town. I only hoped that they would be understanding when we explained that we had successfully put an end to plague once and for all. Maybe I was getting a little ahead of myself. Unsure what our next move should be, I suggested that we wait until all of the lights were out in the house before we tried to enter. We were already surely grounded at the very least. No sense in rushing it now. Josh agreed, and we hid ourselves below the railing of the front deck that wrapped around the cottage in the cave. We would take turns looking up to check on the lights, on occasion, but it would be sometime before the house fell into darkness.

It was nearing ten O’clock when the whole cavern around us fell dark. Somehow, the lights from inside the house had generated the glow through the entire underground, though it made little sense to entertain such a thought. We started to climb the steps that led to the patio when Josh gave a slight jump when the second stepped squealed beneath his foot. We stood as deer in headlights for a moment before feeling safe that we were still unnoticed. My friend looked from side to side as my outstretched arm attempted to turn the doorknob. Of course, that would be too easy, I thought to myself when the knob wouldn’t turn.

We worked our way around the deck, checking all of the windows until we found one unlatched at the far left towards the rear of the house. Josh lifted it slowly and peeked his head inside. He held a hand out for me to hand him one of the torches since the inside of the cabin was as pitch dark as the cavern that surrounded us. Assured everything was clear, Josh slid through the window and I followed in behind him. It struck me how perfectly normal this place looked. Average furniture you would expect to see in an average home.

The pictures that hung on the walls had a similar motif as those I had seen in the various hotel rooms that my family had stayed in during the occasional holidays. There was a lighthouse overlooking an unspecified ocean in one. Another had a pretty landscape of a mountainside, not indifferent from one that may have a cabin such as this one perched upon one of its ledges. No pictures of the family that lives here, though. Only impersonal images of other places, each unique in its own way.

We inspected the perfectly normal rooms as we stalked our way through the cabin, but found nothing that would imply this place held nothing more sinister than a very average family. The kitchen held all of the average foods and utensils any other kitchen would. I halfway expected to find human organs wrapped in transparent plastic when we opened the freezer door, but no. Only very generic frozen dinners with a healthy amount of frost built up on the boxes. Though everything looked right in its place, everything in here felt unused.

The dishwasher was empty, the canned goods in the pantry had dust gathered on top and around, and the condiments were all far beyond their expiration dates. It seemed all of this was a facade to fool prying eyes, but why go to such lengths when this house sat in a location that nobody would easily stumble across? Regardless of how the kitchen appeared, we had found nothing that convinced us this house held anything malicious. That was; however, until we found the basement door.

We had a good idea that Mary would be in the room on the second from which light shone through earlier. Perhaps we were stalling our ascent up the stairway, but we assured ourselves that we only sought to be thorough. Having inspected all of the rooms on the ground floor, the only door we had left to rule out was the one at the end of the hallway. Judging by how long the house had appeared from the outside, that one final door should have logically led back outside. This was why we made no effort to investigate it at first. We may have chosen to check it out in spite of that fact because we were still in no rush to reach the second floor.

Josh swung the door open with little care, being that we expected it to find the rear deck before our eyes. What lay before us was a concrete staircase that led downwards. Any basement to the house should have led back under the building, but this stretched ahead of us. Assuming that we simply misjudged the length of the cabin, we headed down into the black. Our flashlights only carved the slightest circle of visibility through the thick darkness, which was an eerie thing in itself. Josh shook his torch as if to force the presumably weakened batteries back to life to no avail. I handed him my light, but it performed no better than his. Feeling more hesitant, we continued on.

The flight of stairs carried on for far longer than we had expected, covering the distance of maybe three times the height of the house above us. We reached the bumpy concrete floor below and saw the cave that stretched out in front of us. The further we got in, we began to notice a warm glow of light in the distance. Josh clicked the button on the torch, effectively snuffing our own light out. We arrived at the mouth of a large, circular cave which was slightly lit with two candles on either side. In the center lay a large bed with a small table on its right. There was someone under the mossy, green blanket that was sprawled across the mattress, and they looked to be sound asleep.

The closer we got, the less human the shape under the blanket appeared. The shape that formed beneath the sheets was almost snake-like, but lay the length of a tall person. I covered my mouth when I gasped upon seeing what the table beside the bed held. It was a glass bowl, possibly crystal, that contained two human eyes which seemed to gaze up at us. The mass under the blanket shifted and we heard a scuffling sound behind us. “You don’t belong here!” The squealing voice of Mary Marble chanted from the darkness.

I whipped around to see her charging at us. She had an expression on her face, far wilder than the blank stare she normally wore. Her brow wrinkled and contorted into a violent sneer. I pulled out my baseball bat and Josh grabbed for his machete. I was terrified but prepared to stand my ground. She was almost on top of us when her clothes just dropped to the floor as her body evaporated from inside them. I darted my eyes around the room as the thing on the mattress looked to be attempting to wrestle itself out from beneath the blanket. Josh and I spread out as we cut our eyes from the bed to the open room and back again.

I heard a sound as if somebody was raising the volume on the tv as one of the characters on the screen began to wail. The sound belted from right beside me until I found myself pushed to the ground. The force behind the shove was strong. It felt as if a heavily muscled grown man pushed me to the floor. I saw Josh hit the ground while I picked myself back up. I was swinging the bat back and forth against the air around me as though the wind itself was my enemy. I felt a slap across my face, followed by a punch to my gut. It knocked the wind out of me, and I struggled to stay on my feet. That was when I saw the hand reaching out from the blanket.

The hand looked human in shape, but scaled like a lizard. The long fingers wrapped around the eyes that lay in the glass bowl. I watched the hand as it raised up to place them into the open sockets of the scaly face that raised from the bed to meet them. In all honesty, the face was not unattractive, which was the last thing I needed my mind to latch onto right now. It was equally as lizard-like as the hands, but the shapes and contours of the face were soft and lady-like. My legs were knocked out from under me and I found myself on the cave floor, once again.

I noticed I could actually make out the misty form of our assailant when she came close. It was as if Mary was made of smoke or fog when she struck, but she hit like a truck when she made contact. As I battled the ground to release me again, I saw the lizard woman on the bed whip out from under the sheets. She was fast. Crazy fast. While Mary knocked me down once more, causing my helmet to fly off of my head and roll across the floor, the scaled woman charged at Josh after repelling off of the wall that circled the cave. She was a female for sure. Her upper body wore the undeniable attributes of her gender, though her lower body formed the shape of a giant snake. She was scaled from head to tail, and her open, screaming mouth revealed long and pointed teeth within. The lengthy, flowing blonde hair that blew behind her as she sped towards my friend appeared incredibly out of place and I found myself momentarily distracted by that.

The sharp claws on the lizard woman’s three fingered hands swiped at Josh, tearing through the flesh on his right shoulder. He yelled and swung with his machete. He only swatted the air as she struck again, this time, across his midsection. He winced and buckled to his knees. Mary bore down on me with great force. She punched me in the face and pounded my chest. I tried to fight back, but she was only solid when her blows landed. I momentarily grabbed her fist when it pounded at the same place on my chest multiple times, but it dissolved into smoke before I could return any sort of attack. Josh and I were both bleeding and bruised and showed little hope of surviving this.

What were these things? I asked myself, my mind grasping at straws to make sense of it all. The lizard creature bore down on Josh once more, and everything seemed to move in slow motion before my eyes. She reached back with her clawed left arm and held her right out in front of her. She leapt at the roof of the cave and launched herself back towards my friend. My senses blurred momentarily as another punch landed me across the jaw, causing my head to spin. I looked up in time to see the scaled fingers bury themselves into my friend’s chest. He screamed as she rared back and prepared to bury her left hand beside her right when Josh sank the blade of his machete deep into her throat.

A guttural and gargled scream echoed the wall of the cave as the misty form of my attacker became solid again. I grabbed for my baseball bat and tried to swing, but she jumped off of me and sprinted in long strides to the creature who was spewing thick, dark blood onto my friend. The wailing scream made my ears burn and I clutched my hands around them, dropping my bat to the ground. Mary, now fully solid and seemingly human in appearance, pulled the scaled woman off of Josh and pleaded for her not to leave her.

The creature’s wails finally faded into spluttering coughs and she fell silent. Mary released a frantic yell filled with rage and jumped on to Josh, who had been clutching at the holes on his chest and writhing on the ground. She wrapped her solid human hands around his throat and screamed and bellowed in his face, saliva dripping from her mouth. He tried to fight against her, but she was proving so much stronger than she was before, now that the fury consumed her.

My baseball bat broke in half across the back of Mary Marble’s head after I swung it with every single ounce of strength I had left. Her naked body crashed to the ground and stopped moving while bright red blood leaked from where the now splintered wood had made contact. I crawled over the ground to where Josh lay, showing no sign of movement. I shook him wildly as though he were just sleeping soundly. It wasn’t until I slapped his face that his eyes flew open and he grabbed my hand to stop me from finishing the next swing of my wrist. He coughed and shifted on the ground, attempting to pick himself back up. I was relieved to see that the blood leaking from the right side of his chest had already begun to slow. It seems the wound was a lot more shallow than I had feared.

Thankfully, the football pads had prevented the claws from reaching too deep. The cuts across his midsection were shallow, but the ripped wound across his shoulder looked bad. He was having trouble lifting his arm, leaving me to assume some tendons or ligaments may have been severed. As we sat side by side trying to collect ourselves, I let out a long sigh. “You look like shit.” Josh said, pointing at my face with his good arm. I rubbed my hand across my face and winced from the pain of the swollen flesh beneath my finger tips. “You think she’s dead,” he asked, nodding towards where Mary lay still. “God, I hope so.” I replied with a chuckle.

Some minutes passed while we sat, before convincing our weary bodies to raise up from the floor. We grunted and moaned with shared agony as we lifted ourselves to our feet. We slowly staggered in the direction of the mouth of the cave, when we heard another scream filled with primal rage from behind us. Before I knew it, I was knocked back down to the ground. I spun in place to see Mary swat her hand at Josh causing him to fly across the room and slam into the concrete wall. She turned back to face me and I watched her face distort into something barely human. Her mouth fell open and unhinged in a way that splayed it wide open like the mouth of a shark. Her brow folded into a scowl that formed a mass of wrinkles where it met her nose and her eyes had turned a shimmering black. Dark tears flowed from them and they pulsed as though throbbing, black hearts beat behind her sockets.

She jumped on me and wrapped her fingers around my neck. Her fingernails pierced my flesh and I could feel blood leak out around them. She squeezed my throat and pounded my head against the ground. I felt my body growing weaker with every thrust and my head was becoming light. My vision faltered and I could feel the life draining from within me. I was on the verge of allowing the darkness to take me in, when I saw the rusted tip of a machete sliding towards me through Mary’s throat.

The contorted face before me softly formed back into the image of a cute and wide eyed young girl. As her eyes morphed back into human, though dark, eyes, she shifted her gaze to meet mine for the last time. Reality seemed to split before me, and I found myself sitting at a table outside an elegant little cafe on the side of a city street. I didn’t recognize the city, nor did I understand why I was holding a strange, reflective black rectangle in my hand. I noticed there were buttons on the side, so I pressed one with my forefinger. The thing I held was apparently some sort of device which had a screen which showed me the time and date. August forth, twenty twenty one. I looked around me to see the bustling streets filled with people coming and going as extravagant looking vehicles sped by on the road. The voice of a pretty, brunette waitress asked me if I’d like a refill, to which I absentmindedly nodded. I watched her fill up my tea cup and wander back into the pleasant cafe.

I caught my reflection on the window and found that I appeared dramatically older than I knew myself to be. I was balding and what hair remained was grey. I had a thick beard and wore rimless glasses. I was mesmerized by my foreign reflection, but was brought back to my senses when I noticed two cars slam into each other behind me. I spun my head around in time to see a red sports car careening toward me. Everything went black for a second until I found myself back in that cave. My body was numb and a depression began to wake inside of me. I felt like I would never be happy again, as if the preview of my own death, though many years from now, awoke something hollow within.

Maybe I had harbored a sort of disillusioned idea that I would live forever and feeling my own inevitable end for one brief moment was enough for some sort of darkness to take hold of me. I raised my eyes to see the last glimmers of life leaving the cute, wide eyed girl in front of me. Her hands slid down my neck and she fell against me, resting her head on my shoulder. I shook my head, momentarily losing the memory of the vision Mary’s gaze had shown me. The darkness had released its grip on me when I looked up to see Josh standing before me, still wearing a very cracked helmet and holding a bloody machete in front of him. He was panting heavily and wearily asked, “Are you ok, man?”

It would be way past midnight by the time we found ourselves back in the forest above. After leaving the cave and climbing back up the stairs to the pleasant underground cottage, we took the time to look around the rest of the house to ensure there would be no more surprising visitors on our return trip home. Once we had inspected all of the rooms, we cleaned our wounds in the second floor bathroom before going any further. Mary’s bedroom was surprisingly typical of any teenage girl and the cute, stuffed unicorn laying beside her pillow sent a pang of guilt through my chest. All of her clothes were neatly folded or hanging in the closet. Her bedspread and clothes all shared the same dark colors, as did the curtains on the window we had previously seen light shine through.

Even though we had no doubt that she had, indeed, been responsible for the awful events that had consumed Grady these past months, I still felt the weight of having taken part in ending her life. This experience would surely take a toll on my psyche in the years to come. We cleaned our wounds in the upstairs bathroom, before starting our trek towards the rope that hung from the hole in the forest floor. We left the light on in Mary’s room to light our path, as we had forgotten our torches and backpacks in the cave below. We briefly considered going back for them, but I had no desire to return to that place, and Josh showed no signs of disagreeing. Climbing the rope was no easy task as we were both fatigued and Josh only had one fully functioning arm at the moment.

I went up first and tried to pull my friend up as well as I could while he locked the rope between his feet and shuffled himself up while holding on one-handed. It took some time, but we found ourselves back between the darkened trees. Though we had little idea which direction to go, we didn’t spend long trying to find our footing and after walking for what felt like hours, we were surprised by darting lights ahead.

The search party that had apparently been formed sometime before midnight, found us bloody and beaten some time around four am. As it turned out that we had not remotely been walking in the right direction and likely would have come across them some hours before had we chosen the correct path. Clearly, directions were not our strongest attribute. My parents rushed to me and wrapped their arms around me as Jim Crawford, the resident sheriff of the city of Grady, led us out of the woods. Josh was greeted by his mother and older brother, Richard. They excitedly embraced him until he winced and grabbed his shoulder. His father was apparently still inside the forest, but had not allowed his wife and son to enter out of fear of what they could be facing given the tragedies of late.

We were taken to the hospital in one of the neighboring towns and were patched up while officers questioned us on the night’s events. We told them everything as we remembered it, though they did not appear to believe a word. A thorough investigation of our claims was conducted over the following days and, upon finding the quaint cabin that lay beneath the forest above, they could not deny our story to be, at least, mostly factual. Some government officials came into town in the weeks ahead and roped off the woods to conduct their own investigation. We would never hear the results of what they may or may not have found, though. Even if we hadn’t moved away from Grady before the month came to a close, it was quite certain they’d never share their findings with the public.

Josh and I still stay in touch to this day. He became obsessed with researching supernatural phenomena, and even hooked up with a group of people who shared his shared interest, each with equally as mind boggling personal experiences as ours. I’ve been to some of their meetings, and with their permission, I have used their experiences in my writings. Malcolm, an especially nervous and paranoid, but well meaning associate of Josh, claims that what we saw in the woods that day was something named Lamia, and that Mary was possibly some sort of wraith who was perhaps raised by the scaled creature. These were only theories, of course, but it was something to give us some semblance of closure.

Josh and I took a road trip a couple of years back. We returned to Grady to find that the tragedies had, indeed, ended the day we walked out of those woods under the crescent moon. It made me proud of what we did that night, looking at the happy and carefree children running through the park and laughing in the playground, though I still bear the burden of the lives we took that night. I still visit my therapist regularly, though she is convinced that my memories are no more than a manifestation of some underlying mental suffering that my subconscious created. Though I still battle with the memories of my childhood in Grady, life has been good, and I have made a happy life with good friends and a loving wife.

I’m supposed to meet with a new publisher in the city tomorrow at a cafe named Blue Moon. It has a good reputation and is known for serving quite the delicious cup of hot tea. I do believe I’m going to cancel that meeting, though. I pay little attention to the date as my work is done from my office at home, and I don’t always have a strict schedule I have to meet, but according to my cell phone, it’s August third today. Perhaps I can reschedule.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "52de4f3a-7ec4-464f-a6e0-aa77c881cc53",
                    ThreadTypeId = 1,
                    Title = "Traveler In The Dark",
                    Content = @"
The glow of the setting sun entrenched the peaceful Italian countryside in a blazing red haze. The small rural highway was surrounded by rich forests currently going through the enchanting transformation of autumn. The picturesque landscape was unfortunately lost on the highways sole traveler Chiara Gallo. She sat behind the wheel of her car with a complete lack of interest in the stunning world around her. Her family reunion had gone exactly how she expected it would. It was 2 days straight of “Chiara when are you getting married?” “You aren’t getting any younger so you might want to start thinking about children.”

She let out a frustrated sigh. It wasn’t that she hadn’t tried to start a family however her dating life was in shambles and when it came to children she didn’t know where to start. “I just don’t understand them.” Chiara said out loud to the crisp country air. Speaking her thoughts out loud was something of a bad habit she had picked up over the years. Living alone was a quiet life and sometimes you needed someone to talk to even if it was yourself. “Add that to the list of things that make me so dateable.” She shook her head and turned up the radio wanting to drown out her whole weekend.

The sun sank beneath the clouds sending a shadow over the land. “Great just what I need. They wouldn’t let me leave early even though I made it clear I had a long drive down to Florence. Now there is no way I’ll get back before late at night. Damn it I just want to rest!” As she turned a large corner her radio turned to static almost deafening her in the process. She looked down at the console desperate to change the station and save her ear drums. She looked back up and was completely taken by surprise when she saw a person walking along the side of the road, right in her path. Chiara slammed on the breaks and swerved her car, sending up a silent prayer that she missed whomever it was walking there.

She managed to stop her car and pull over to the shoulder. Though she didn’t feel a bump or crash she couldn’t be sure with the speed of the incident. She looked into her rear view mirror and was shocked to find no sign of anyone in it’s view. “There was someone there, I know there was.” She turned around and examined the area behind her. Just barley visible at the bottom of the window was a little girl knelt over with her hands over her ears. Chiara jammed the car in park and ran out to the child.

“Oh my god I am so sorry, I didn’t see you. Are you alright? Are you hurt little girl?” The little girl shook her head and stood up slowly. She was extremely small, maybe as young as five or as old as eight, kids were such a mystery to Chiara. She wore a white summer dress and held an old fashioned rustic doll. “Sorry if I scared you. I hope you and your car are alright.” The little girl spoke, looking very shy and guilty. Chiara had to let out a small laugh. She had just about turned this girl into a pancake and this girl was apologizing to her? “Oh don’t worry about that, I’m more concerned about you. What are you doing walking the road like that all by yourself?” Chiara inquired with legitimate concern.

“I’m making my way to see my family.” The child said with more confidence. Chiara was completely stunned by the casualness of the statement. “You’re miles from anywhere darling and way too young to be out by yourself.” The little girl’s cheeks flushed and she put on her best pouting face. “Hey I’m not so little, I can take care of myself.” Chiara let out another small laugh. “I’m sure you can but it’s getting late and I can’t simply leave you out here all alone. Tell you what how about I give you a ride to the nearest police or bus station?” The little girl’s eyes filled with fear all the sudden. “The bus station is fine but no police. They always make things more complicated then they need to be.”

Chiara didn’t feel comfortable leaving this little girl somewhere on her own, even at a bus station. However since she almost ran this girl over maybe the police may not be the best place for her. “Ok you have a deal little miss.” Chiara got in her car and waited for the little girl hop in the front but the door didn’t open. She looked out and there was her would be passenger looking nervous and uncomfortable staring at the door. “Oh it’s ok I won’t hurt you, please come on in.” The little girl made a large childish grin and hopped in the front seat. Chiara smiled down at her. “There you go, see I promise not to hurt you and you promise not to hurt me. Deal?” The little girl simply laughed at the silly notion.

As Chiara began traveling down the highway again she was amazed at how dark it had already gotten in her short time off the road. The light faded rapidly as if it was fleeing the growing darkness. Chiara, whom had never been much of a night person, felt a chill running down her spine. She looked over at her passenger who seemed as happy and comfortable as she could be. “Weren’t you worried about walking around at night in the middle of the forest?” The little girl merely shrugged. “Not at all I love the night. I haven’t been afraid of the dark in many years.” Chiara almost laughed at the idea of a girl her age using the term many years.

As they drove on a strong stench began dominating the air around them. The smell was horrible almost like rotten meat but with a sense of stuffiness to it, like a room that hasn’t been used in years. Perhaps there was a dead animal on the side of the road but if so shouldn’t the smell be dying down? Chiara tried to take her mind off of it with small talk. “So where are you headed to if you don’t mind me asking?” She asked her passenger. “I’m heading to Rome for a gathering of my people.” The little girl said and there was a tinge of excitement in her voice. “Oh I’ve always wanted to visit Rome but never found the time. Is your family from Rome?” The little girl shook her head and left it at that.

Chiara started to dwell on the girls accent. It wasn’t Italian, that was for certain, it sounded east European but she couldn’t quite narrow it down. Chiara gave a shrug and kept on the conversation. “I just got back from a family reunion myself.” The little girl looked to her and nodded. “Yes sorry it didn’t go so well. Families can be such a pain can’t they?” Chiara was a little shocked. “How did you know that it didn’t go well?” It was the little girls turn to shrug. “Something in your voice. You sound like someone who has been torn down by your family but at the same time you know that they are partially right.” Chiara got another chill. “Wow you are a very astute little girl. That happens to be right on the money. You must do well in school.” Her passenger simply chuckled and looked back out the window.

Outside a thick layer of white fog began to roll in seemingly from the forest surrounding them. She didn’t remember seeing calls for fog in the weather forecast. Suddenly the previously scenic forest was now ominous and foreboding. To make matters worse that smell had not gone away. Quite the contrary it had grown stronger if possible. “So do you have any siblings? A sister or a brother?” Chiara asked trying to calm herself down. At the word brother the little girl’s entire demeanor changed. No longer was she calm and smiling. Her face narrowed and she began to aggressively pant under her breath. She looked like a wild animal in a deep snarl. “I had a brother but no more. We do not speak of him and when we do it is in cursing his name. He and his friends hurt my family. I will never forgive him for that.” Chiara could sense that this subject was only going to garner more animosity from her fellow traveler so she dropped the subject.

“You must be excited for this reunion though. That must be something.” The little girl’s face returned to normal and she nodded. “Yes the gathering, I am most excited for that. There has never been a collection of our people like this, not in many lifetimes.” Chiara was a little unnerved by the usage of lifetimes and our people but she didn’t want to delve deeper. She remembered how family gatherings felt when she was younger and smiled. The big banquet with only the freshest of foods. Being allowed her first sip of wine by older relatives. She would spend the day playing hide and go seek with her many cousins. Then the night would end with a big bonfire, roasting marshmallows and telling ghost stories.

“Oh there will be plenty of fires, I suspect before our gathering is over and plenty of new ghost stories for people to tell.” The little girl said with a big grin on her face. Chiara was shocked and then succumbed to embarrassment. “I’m sorry I tend to think my thoughts out loud, sometimes I forget I’m not alone.” Chiara shook her head not believing she had thought out loud in front of this little girl. But had she? She didn’t remember feeling herself do it this time. All around the road the fog grew thicker making it harder to see. A gust of wind began swaying the trees back and forth almost as if they were alive. What she wouldn’t give to be in her home right now soaking in a tub with a glass of wine. She sighed as now she couldn’t even go straight home. She had to deal with this little girl first.

“If I’m too much of a bother I am more then fine to walk the rest of the way. I’m sure that you would rather be at home resting in the bath.” The little girl said sounding a little disappointed. Chiara was floored, this time she knew she didn’t think out loud. She looked over at her passenger and found her simply staring at her. She looked at her with those puppy dog blue eyes with pupils almost too big for her face. It should have been cute but Chiara could feel something underneath. It was as if those eyes had hidden layers to them, like you could drown in their endless depths. Whatever the owner of those eyes wanted you had to obey them or lose yourself in that stare. “No don’t be silly I can’t leave you there on the side of the road. Not after dark with this fog out there like that.”

Chiara nodded and turned back to the road, feeling a little light headed after her eyes left the little girls. Despite the fact that she had broken her gaze, she could tell the little girl had not broken hers. She could feel those eyes staring into her, never blinking, never veering. Out of the corner of her eye she could see the little girl grinning. It was not the grin of a happy girl but the grin of a beast, a predators grin. The stench blasted in her nose and it took all she could manage to not let go of the steering wheel and plug it. “How is she standing it?” She thought to herself. “There are worse things in the world then simple smells.” The girl said flatly. Chiara almost swerved off the road. This was the third time this girl pulled something straight from her mind. She began to feel very uncomfortable with her in the car and wanted nothing more then for her to be gone.

“Don’t you ever get tired of being alone Chiara? Don’t you want to be part of something more?” The little girl said in an almost hypnotic tone. “What do you mean?” Chiara asked back shaking well she did so. “I mean there are so many people like you out there, lonely and desperate for connection. You struggle in vain for I know of a place where you could have all the company you want. A people that would accept you with open arms, with no judgement or fear. My family we have ways to connect that you can’t even imagine. You would never have to be alone again. Doesn’t that sound wonderful?” The scary thing was to Chiara that it did sound wonderful. She could picture herself seeing her family not in fear or frustration but genuine excitement. A family where she could truly just be herself. “I could give that to you Chiara and all you would have to do is ask.” The little girl’s eyes were almost luminescent in the light of the moon. She could feel herself getting closer and closer to her in the seat.

From deep in her mind fear and rational thought burst out. “Don’t let her near you. Don’t let her sway your thoughts. You need to get rid of her and quickly!” Her mind screamed. Chiara shook her head and clear thoughts returned. The voice was right, there was something wrong with this girl and she had to go. “Say we are coming up to a service station are you hungry at all?” The little girl froze for a moment then finally blinked and turned from Chiara. “Well I am quite thirsty.” Chiara smiled and nodded. “Then it’s settled we will stop at the store and get you whatever you want and it’s my treat. Anything to get off this foggy road right?” “And away from you.” She thought praying the girl wouldn’t pick it up.

A few minutes later she had pulled in to the station and parked out front. “I’ll wait in the car.” The little girl said flatly. Chiara’s blood froze for this did not fit her plan. “Oh but I don’t know what you like. Here I’ll give you the money and you can get any drink you want. Even if it’s something you can’t have at home, I promise not to tell.” The girl wrinkled her nose and thought it over. “Just go inside. Get out of my car and leave me please.” She prayed in deeply inside of herself making sure not to voice it. The little girl finally shrugged. “Sure I won’t be long just wait here for me.” Chiara smiled. “Sure thing cross my heart and hope to die.” The little girl smiled at this and hopped out of the car.

Chiara had reached for the gear shift when she saw that the little girl had left her doll on the front seat. That stupid doll with it’s old fashioned setter type dress. The thing looked ancient and was like an antique “Oh wait dear you forgot your doll.” The little girl swung around with a big grin on her face. “Oh thank you so much I never go anywhere without Heidi.” The little girl took the doll and cuddled it to her. Despite wanting to be rid of her, Chiara suddenly felt a great wave of guilt. In all this time she had not asked the little girl’s name even once. “Oh I’m so stupid I never did get your name darling.” The little girl laughed. “Oh how silly of me it’s Gracie. Now I’ll be right back please wait here for me.” Chiara pointed down at her spot and Gracie went inside.

As soon as the door closed Chiara put the car in drive and sped off as fast as she could, not giving a damn how it might look to anyone else. She raced down the road afraid to check her mirrors lest Gracie be looking back at her. She felt a little guilty leaving her but screw it. There were a lot of people at that rest stop, let one of them deal with that creepy girl. The more she drove the more she felt like herself. Her heart slowed and began to calm down. That foul stench was gone and even the fog seemed to be lifting. It was as if she had gotten rid of some cursed object.

“Well that’s a horrible thing to say about a little girl like that.” She said out loud no longer having to be embarrassed by it. “Hey no disrespect but that girl was not right. Besides I got her a little further down the road, I’m not running a limo service here.” She nodded her assurance that she was in her right to do what she did. However as her odometer grew so did her guilt. Sure she may have been strange but she was still just a little girl all on her own in the woods.” She felt guilty alright but something about the word woods seem to have struck her. This road was almost nothing but forest for miles going both ways. Where did that little girl come from? Had she been walking all day on that sun parched highway? If that was the case then how come she wasn’t hungry at all and only thirsty. Again the word thirsty struck her with a type of dread. It was as if her subconscious had picked up on something she hadn’t.

“Oh what was there to pick up? She was just an ordinary little girl and I abandoned her. The poor thing was so helpless and shy she wouldn’t even get in my car until…” She almost jammed on the breaks in sheer shock. “Until I invited her in.” Suddenly all the pieces in her head clicked together at once. Suddenly she had a clear and horrifying picture of exactly what was going on. “I couldn’t see her in my rear view mirror.” A barrage of the things the girl had said exploded in her mind.

“I haven’t been afraid of the dark in many years.”

“No police they always make matters more complicated then they need to be.”

“I’m heading to Rome for a gathering of my people.”

“Our people.”

Chiara’s heart pumped wildly, threatening to burst from her chest. She had heard the legends but never thought they were real. And yet here she was, she had sat next to one in her car. ” She was a creature of the night, the living dead, a….” Chiara wouldn’t let herself say it, out loud or otherwise. “You shouldn’t have driven away.” A voice sounded from outside her driver’s side window. She looked over and there was Gracie floating outside her car despite it pelting down the highway. She let out a loud scream and looked back to the road. The highway was turning and she didn’t have time to turn with it. She jammed on the breaks but not fast enough to stop her car from slamming into a telephone pole.

Her airbag deployed but didn’t stop her from smashing her forehead off the steering wheel causing a fresh rush of blood to erupt. In her shock she had forgotten her situation. She slowly looked up from the bloody air bag and could see her car smashed against the pole. Small flames were coming from the engine. This shocked her back to reality. “I need to get out of here encase it blows.” She turned to her door and was put right back into a state of shock. For waiting outside her car, lit up by the light of the flames, was Gracie.

Whatever glamour the girl had cast before was now long gone. She was seeing the Gracie underneath, the true Gracie. Her skin was a dark and yet dull grey. Her hair perhaps was once blonde but years of no sunlight and sleeping in the earth had turned it a jet black. Her hands bore long and crooked fingers, each tipped with a long claw. Her skin had been rotting away in several areas and Chiara understood what that foul stench had been. Two globous obsidian eyes stared out through skeletal sockets in her head. Her mouth was open giving way to two large incisor fangs. The fangs were too impossibly large for her tiny mouth, giving her the appearance of a saber tooth tiger. There was no longer any doubt in Chiara’s mind, Gracie was Nosferatu, the vampyre.

Gracie began panting and snapping her jaws hungrily. “The blood, dear god she can smell my blood.” At the word blood Gracie began inching forward towards the trapped car and Chiara began to panic. “No you can not come in! I forbid it. I revoke my invitation.” Gracie simply let out a laugh that echoed through the forest. “Oh it’s too late for that my dear. Once given it can never be taken back.” Gracie continued her march toward her. Chiara panicked, she began searching her car for anything that might help. A crucifix, a rosary, even a bible but there was none. Her mother always traveled with a rosary and yet she never did. Chiara tried to climb out the passenger side door however it was in vain as her leg was trapped in the wreckage.

“Aww do you need a lift?” Gracie asked. She was now right outside the window. She grabbed the door and ripped it open leaving no barrier between them. “No please don’t! You promised damn it. You promised not to hurt me.” Chiara pleaded. Gracie simply smiled and tilted her head to the side. “Did I?” Horror pulsed through Chiara. Gracie never did promise her did she? Chiara had promised and Gracie merely laughed. Gracie smiled wider, pulled Chiara’s leg free and threw her to the pavement. Tears rolled down Chiara’s face as she tried to crawl away despite the pain. There was a small whooshing sound and a moment later Gracie was on her back.

“You know with your leg like that you remind me on someone I once knew. He ended up hurting my family but you won’t. You will help replenish what has been lost. You will join my coven, my family. Gracie’s decaying arm came down in front of Chiara and she had to hold back a gag at the rotting stench. A long claw dragged across Gracie’s wrist causing blood to pour from it. The blood that poured out was not dark red but pure black like her eyes. Chiara could even see tiny maggots squirming in it’s depths. Realizing what Gracie was planning to do Chiara jammed her mouth shut and screamed internally for salvation. Without warning those two cat like fangs pierced her throat and Chiara was forced to let out a loud scream. Gracie shoved her wrist into Chiara’s mouth and despite her struggles she could feel the blood streaming down her throat.

The taste the blood left was horrible beyond measure. It was as if that terrible rotting stench was personified in this black ooze. She could feel it making it’s way through her body and into her veins. She could actually feel the unholy abomination begin to change her from the inside out. Her body felt cold and devoid of all life. “This is what a corpse would feel if they could think.” The thought chilled her even further. All rational thought was seeping out of her body like air out of a pierced balloon. All she was left with was an insatiable sense of hunger bordering on madness. Gracie removed fangs from Chiara’s throat and flipped her over. She smiled down her with her horrifying grin. “Welcome to my family sister.” Gracie wiped her mouth of Chiara’s blood and clenched her fist, sending down a couple of scarlet drops. The drops entered Chiara’s mouth and to her surprise it didn’t taste revolting. In fact it tasted delicious, like that first sip of wine from when she was a child.

The next morning officers examined the wreckage of Chiara’s car. The car had been stripped of it’s licence plates and insurance papers. Despite the wreck and deployed airbag there was no blood left on the scene. That had been gleefully lapped up the night before. To the officers the case seemed pretty clear. The car must have been stolen by some drunk hoodlums from a parking lot along the highway. They had crashed the car and left it there, taking the licence plates to make it harder to track. The police left the scene and waited for someone to report the car stolen.

They did not investigate the matter any further and that is a shame because had they done their research they would have discovered that this was not an isolated incident. For all over Europe several missing vehicles turned up along the sides of deserted roads. With them came missing persons, though in truth they were not missing, they were traveling. For the dead were indeed traveling, converging, gathering. Their eyes were set on a single destination. The purpose of their pilgrimage? Revenge.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e6409d32-32ff-4186-9c3f-d63523794eae",
                    ThreadTypeId = 1,
                    Title = "Have You Seen the Blue Man?",
                    Content = @"
Honestly, I never wanted to relive this part of my life again, but something has happened that has forced me to do so. I don’t even remember much, the entire part of my life when the event happened seemed like a blur, like it wasn’t even real – for the longest time I even thought I had dreamt it up, or maybe I knew it was real but refused to entertain the idea out of fear alone. I was a fool to think I could have just erased something like that from my past; it looks like my past has finally caught up with me. I’m going to recall my experience from when I was a child, I’m going to try to remember as much as I can, but like I said, much of it is a blur. This surreal experience terrorized me for three years of my life, beginning when I was seven years old; I called this experience, ‘the Blue Man’.

Everything began with a door. One night I would go to sleep, tucked behind my big pink and purple bedsheets, my blue nightlight in the corner of the room, illuminating the closet door that I was always so afraid of. I dosed off to sleep after my parents kissed me goodnight, nothing seemed amiss. I had quite a vivid dream that night; usually, dreams are forgotten quite easily, you remember them for a few minutes upon waking up and then they fade away, never to be remembered again, there are only a handful of dreams that stand out enough to be remembered, and this was one of those dreams.

I remember sitting in a field, similar to the wheat field behind my house. I was wearing the same pajamas I had on when I went to bed. It was dark out, heavily overcast and the wheat around me seemed to flow steadily in a slight but calm wind, I remember how deafly silent it was. In the middle of a field, just a few feet in front of me sat a single black, wooden door. A knock came from the other side, three knocks in quick succession. Puzzled for a moment, I stood up and slowly walked over to the door. I peeked around the door frame, confused to why someone would knock on a door with no walls separating the other side – nobody was there. Another knock echoed through the field, three more quick knocks with a voice following this time; the voice was calm and soft, like the wind, yet at the same time it made me uncomfortable.

“Hey,” spoke from the other side, slightly muffled behind the door, “can you let me in? It’s cold out here.”
That was all the voice said, and without thinking much about it, I touched the black doorknob and turned it slowly, and as I watched the black door slowly open, cold air blasted in from the other side. The once still and quiet field was hit with a rush of cold air, I stumbled back a few feet as chills rippled down my spine and the crops of wheat shook. As the rush of cold and violent air subsided and the field once again grew still, I saw as the opened door had nothing on the other side; nothing but the other side of the field with the same overcast sky. That was when I was pulled from my dream, I woke up in my room, I remember being very cold despite being tucked under my sheets. I turned over to see my alarm clock, the sunlight from the morning sky outside peeked through my window and hit my face. I sat up and quickly noticed that my bedroom door was open, I had sworn my parents closed it the night before after tucking me in, they always did – if only I had known then what I had unintentionally invited into my life.

Nothing much of significance would happen that day, I would go to the kitchen and meet my parents and they would make me breakfast before sending me off onto the school bus. My mother would make an offhand comment about hearing my door slam open during the middle of the night, accusing me of being awake way past my bedtime, I wouldn’t think much of this until later. I would go to bed the next night, same as usual, and make a mental note when my parents closed my bedroom door this time. Feeling reassured, I would pull my blankets up to my neck and stare at the ceiling until I grew sleepy – except, I couldn’t sleep that night. Something would catch the corner of my eye, my nightlight quickly flickered as if something had walked by it. I sat up and studied my closet, it was cracked open ever so slightly.

I narrowed my eyes to adjust to the darkness and saw the nightlight had illuminated someone sitting inside of my closet, peering out of the tiny crack. My heart sank and my blood turned cold as I stared whatever was in the closet in the eyes, I felt paralyzed and was unable to move, to even scream. The closet was slowly pushed open and whatever was in there revealed itself. Illuminated from my blue nightlight was what looked to be a man, but I knew instantly it wasn’t human – it resembled a man, but something was off. Its face was over exaggerated in every way possible: its eyes were human, but they were too big and wide, I don’t remember ever seeing any eyelids, and its pupils were far too small; its nose was long and pointed, and its hair was dark and slicked back. Yet the feature that stood out most was its smile – it was permanently smiling, mouth reaching from ear-to-ear, and I could have sworn it had far too many teeth in its mouth to be human.

The creature slowly stood up, never breaking eye-contact with me, and never losing that damned smile. As it stood, it reached the top of my closet and had to hunch over – my closet door was over six feet tall, I knew instantly its size was not of a normal man’s. Still illuminated by my blue nightlight, the creature then began to wave its arm forward, as if motioning me into the closet with it. I didn’t move a muscle, instead, I stayed paralyzed in my state of shock, staring the creature in the eyes the entire time, terrified to look away. It continued to motion forward, never letting up – it stood still with the same arm motion and the same facial expression for so long it began to resemble an animatronic, everything about the creature’s appearance was surreal, bordering on real and dream-like. I don’t remember how long we stared at each other for, but I remember blinking one moment, and upon opening my eyes it was gone, my closet door closed.

I didn’t sleep that night, I was too terrified to even move from my sitting-up position. I kept staring at the closet until the morning sun once again hit my face, and even then, I didn’t leave my bed until my father came to get me up for school. I knew I would have to sleep eventually, but I was scared to let it out of my sight – unfortunately for me, the creature would return again that night, and the night after, and the night after that. I was stuck with it as it invaded my room every single night for three years. My seven-year-old mind began referring to it as, “the Blue Man”, whenever I would see it, for the simple fact that it appeared blue through my nightlight. Perhaps there was more to why I called it that, maybe it was because it brought me an overwhelming feeling of grief, angst, and sadness throughout those three years – I didn’t feel like myself, I felt like a zombie, and looking back at it as an adult, I don’t remember much of anything in those three years, except for my experiences with the Blue Man.

The next night, I would hesitantly go to bed, begging my parents to stay and wait for me to fall asleep, telling them I didn’t want the Blue Man to come out of my closet again – they did what all parents would do when confronted with a “monster in the closet” story, they told me he wasn’t real, and to go to sleep. They left my room after tucking me in like always, closing the door behind them. I took in a deep breath and stared at my closet, anticipating it to open once again, but it never did. I almost got some sleep that night, but the Blue Man didn’t go anywhere, instead, he showed up somewhere new; I turned away from the closet for just a split second to see him hunched over in the dark corner by my bedroom door, the back of his neck touched the ceiling. He locked eyes with me once again, and slowly began moving toward me. As he moved out of the dark corner, he changed from a dark silhouette to a blue figure once again; he moved like a puppet on a string, his legs and arms moved unnaturally, like they were being controlled by a string. As he moved, he wheezed like a heavy smoker, like something was closing off his airways. He limped his way over to me while maintaining that same wide-eyed grin, all the way until he made it to my bedside. Once again, I couldn’t scream, move, or do anything, I stared the Blue Man in the eyes until finally, my lack of sleep the night before caught up with me and I couldn’t keep my eyes open any longer – I fell asleep as he watched me.

I had a horrible dream that night. I found myself in the wheat field once again, still winds and the same overcast sky from before. Except this time, instead of a door in the middle of the field, lied an old oak tree, with a single noose hanging from it. The winds once again picked up and in the blink of an eye, a man appeared with the noose around his neck. His lifeless, pale body swung back and forth in the wind. I looked away in horror and curled up into a ball in the middle of the field until the wind stopped. I heard footsteps coming closer to me in the field, but I refused to look up, it was when I felt something wrap around my neck that I woke up in a panic. It was morning, daylight once again hitting my face, but my room once again felt cold. I had trouble breathing for the first few seconds, my chest felt heavy and my neck felt hot. I looked around my room to find nothing – the Blue Man was gone.

Once I regained my breath, I did what every small child would do in that situation: I began to scream and cry until my parents rushed into my room to comfort me. As they sat by my bed and tried calming me down, I’ll never forget the look on my mother’s face as she caught a glimpse of my neck – she quickly placed her hands on my neck and showed my father. They looked confused, then panicked. My mother called the doctor’s office that day, apparently, I had a noticeable bruise around my neck, wrapped all the way around my neck like a ring. My parents took me into the doctors that day, and I remember how exhausted I was. I don’t think my parents got many answers from the doctor, they seemed to leave pretty disappointed and with more questions than answers.

I remember that night my mother would sit by my bedside until I fell asleep – it was comforting knowing she was there by my side, I felt like the Blue Man couldn’t reach me if she was there, but that feeling didn’t last long. I dozed off to sleep, but I woke up in the middle of the night to find she wasn’t there, I panicked. She must have gone back to her room after I had fallen asleep. I looked around my room with sharp and panicked movements, terrified to find the Blue Man again, but he wasn’t there. I took a deep breath, mentally preparing myself to leave my room to go find my parents. I swung my legs off my bed, and when I looked down at the floor, I saw his face, looking up and smiling at me as he lay under my bed. I threw myself back under my covers and covered my face. I heard him slowly pull himself out from under my bed and stand up, all while making those disturbing wheezing sounds – I knew he stared down at me the entire night, but there was nothing I could do about it, and I was too scared to turn around to look.

This continued for so long, and I just forced myself to get used to it. I noticed the Blue Man would never make physical contact with me when I was awake. He would just stare, always in different places of my room so I could never anticipate where to find him next. My sleeping was on and off, some nights I would be too terrified to sleep, but on the nights when I couldn’t take it anymore, my eyes would grow too heavy, I would lose the staring-game, and I would fall asleep, leaving him to give me awful nightmares. The nightmares would happen every night, there were too many to remember all of them. Most of them were pretty similar, and they all had the same theme – asphyxiation. Sometimes I would be in the wheat field witnessing a man hanging from a tree, sometimes I would be so deep underwater light couldn’t pierce through, and I would watch myself drown, and other nights I would watch myself get buried alive. Every time, I would wake up out of breath and the bruises on my neck fresh.

The bruises never went away in those three years, in fact, they got worse. The bruises would only appear around my neck. After a few weeks, my parents would begin fighting regularly, they would scream at each other day and night, pointing fingers at each other. I was too depressed to care. For some reason, during the entire three years, I was too depressed to care about much of anything – I was inexplicably sad for a reason I cannot describe, too depressed for any normal seven-year-old to be. I just felt tired, weighed down from the constant visits from the Blue Man, he felt like a never-ending sickness. Every time he would visit, I would grow terrified, that fact never changed, but I no longer dreaded his visits, I just got used to being terrified every night. I would wear shirts that covered my neck at school, but I must’ve not been able to hide it very well, and child protective services arrived at my house one day to take me away – all I knew at the time was that I had to stay with my grandmother for a little while, and I wasn’t allowed to see my parents.

Even at my grandmother’s house, the Blue Man followed. Same as before, he would appear in my room late at night, once everyone else in the house was asleep. Sometimes he would hang from my ceiling, right above my bed; sometimes it would be back in the closet; and sometimes, he would even be outside of my room, staring in through the bedroom window with his face pressed up to the glass. Three years of my life was characterized by being in hospital waiting rooms with constant examinations, yet no answers. I was too depressed to care what was happening to me, and I never really spoke of the Blue Man to anyone. Doctors, child protective service agents, teachers, everyone would ask me questions, but I would always shrug my shoulders and ignore them. My parents and grandmother could barely recognize me anymore, I had gone from a normal little girl to one who had a constant thick cloud of sadness hanging over them – and a ringed bruise around my neck that never went away. I could only imagine how scared everyone was for me, but it all ended when I was ten, seemingly randomly; perhaps the Blue Man grew tired of terrorizing me?

It ended just how it began, with a black door in the middle of a wheat field. One night as I dreamt, I found myself sitting in the field again. I stared at the door, it was wide open with cold air rushing in. I stood up and walked closer towards it, the air pushed against me and made it difficult to move forward. I reached my arm out to close it, but before I could touch the door, it slammed shut with such force the field around me shook from the impact. The cold air abruptly stopped, and I woke up to the alarming sound of my bedroom door slamming shut. After that night, the Blue Man never visited again, and over the next few weeks my bruise would disappear, and along with it, the overwhelming weight of grief and sadness. Just like that, it was all over. There was nothing significant about the day the Blue Man left, it was just random – I still to do this day do not know what caused him to leave – or to enter my life in the first place for that matter.

It is now the year 2021 when I am writing this, I am now thirty-one-years old, a young mother with a son of my own, Isaac, he’s five years old. I had all but forgotten about the Blue Man, pushing it out of my mind out of pure denial it happened. However, last night I was forced to come to terms with reality – that the Blue Man is real. I was working from home, writing up a report my boss wanted in by the next day, when I felt a tug on my sleeve. I looked down to see Isaac, he looked as if he wanted to tell me something. I picked him up and placed him up on my lap.

“What’s up buddy?” I asked.
“Hey mom?” he questioned, “have you seen the Blue Man?”
“Excuse me?”
“The Blue Man, he told me that’s what you called him. He told me he was cold too, so I let him inside.”

My hands were shaking as I parted his long hair away from his neck, to reveal a slight red ring beginning to appear around my son’s neck.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "8725ba9f-490d-460a-b6cf-d19d9c2ecb37",
                    ThreadTypeId = 1,
                    Title = "The Bar at the Edge of Eternity",
                    Content = @"
Don’t ask me how I got there, because I can’t remember. I’d been drinking heavily that night, out on a bender to end all benders. I was angry, and the booze didn’t help. It never did if truth be told, but I couldn’t stop myself. I vaguely recall downing shots in a busy bar and making an ill-advised pass at an attractive young lady who was out with her boyfriend. Needless to say, this didn’t go down too well.

Punches were thrown, and I was forcibly removed from the premises. After that, the rest of the night is something of a blur. What I do remember is waking up in the squalid backroom of a dark pub, my head throbbing and mouth tasting of vomit. My body was covered in bruises, probably as a result of the fight, and my clothes were ripped and soiled.

All in all, I was in a pretty sorry state. What’s more, I had no idea where I was. I didn’t recognise the bar and was sure I’d never visited this establishment before. My first impressions weren’t great. I’ve been in some dives during my time, and this place was amongst the worst.

The couch I woke up on was filthy and the tired wood floors beneath me were suspiciously sticky. Meanwhile, my nostrils were filled with the mixed odors of spilt alcohol, cigarette smoke, and something fouler which I couldn’t quite place.

I fought through the pain inside my skull as I sat up from the chair, struggling to adjust my eyes to the bar’s dark interior. I was sitting at the back of the pub, facing the bar on the far side of the room. The place was quiet, nearly abandoned in fact. There was a faint background noise – a weird and slightly irritating low buzzing sound that reverberated around the room. I couldn’t place the sound and didn’t know where it was coming from, but I decided not to worry about it for the time being.

I slowly walked towards the bar and surveyed the room, confirming that it was nearly – but not entirely – empty. There were only four other customers besides me. A young man sat directly opposite me, glaring menacingly in my direction. He was clean-shaven but had long blond hair fashioned in a mullet, and he wore a shiny red shell suit and white trainers.

It was like he’d just stepped out of an 80’s theme party. I saw a bottle of beer on the table in front of him, but the 80’s guy wasn’t touching his drink. He continued to glare at me, making me feel uncomfortable. I didn’t know what this guy’s problem was, but if he didn’t quit staring at me there was going to be trouble.

In a darkened corner sat a middle-aged woman drinking alone, sipping from what looked like a glass of brandy while smoking from a cigarette holder. She was a red head, dressed in a fur coat, tweed skirt, and high heels. Again, her look wasn’t exactly contemporary, and I reckoned her clothes came from the 40’s or 50’s. I reckoned there must be a fancy-dress party on, or something.

From looking at this lady, I reckoned she probably had a drinking problem, and it had taken its toll, with heavy bags evident under her eyes and her teeth stained yellow, likely from years of heavy smoking. She stayed in her corner, not interacting with the other patrons or looking in my direction. I decided to leave her in peace.

Next, I turned my attention to a couple perched on stools at the bar. They sat in close proximity, the man’s arm stretched across the woman’s shoulder, as they whispered to each other, the woman laughing softly at her lover’s jokes and suggestive comments.

In keeping with the bar’s apparent theme, the couple looked out of their time. The man wore a pin-stripped suit and fedora-style hat, while the dark-haired woman was dressed in a glamorous, shiny cocktail dress. They both looked like they’d stepped straight out of a 20’s gangster movie. The woman was elegant and attractive, and her intelligent green eyes marked her out as much more than a gangster’s moll.

The man had his back turned on me, but I caught the young woman’s eye, temporarily interrupting their flirtation. Suddenly, the man turned to face me. He seemed to be in a mean mood, and I noted the deep scar across his cheek and the barely suppressed rage behind his dark eyes.

“What the hell are you staring at kid?” he spat angrily.

I’m not usually the type to back down from a confrontation, but there was something menacing about this guy, and besides – I had no beef with him and didn’t need the hassle.

“Sorry pal, my bad.” I answered, raising my hands defensively.

“Just watch yourself buddy.” the gangster scowled.

Fortunately, he seemed satisfied with my submission to his ‘alpha male’ status, and so returned to his lady friend, although I noticed how she shot me a coy smile over his shoulder.

Finally, I looked to the barman for the first time, as he walked forward to greet me.

“Good evening sir, what’s your poison?” he asked in a friendly tone of voice.

The barkeep was an unremarkable looking man with thinning grey hair and a pot belly hidden underneath a checked shirt and denim jeans. His brown eyes looked tired and world-weary, although his voice sounded surprisingly kind and welcoming.

“Erm…I probably shouldn’t.” I replied sheepishly, “It’s been a rough night.”

“So I can see.” he said with a smirk whilst looking me over, “But hey, we don’t judge here. By the look of you, I think a large whiskey is in order. Hair of the dog, as they say. What do you reckon my friend?”

I laughed nervously, as the man’s amicable nature slowly put me at ease.

“Well go on then, you’ve twisted my arm!” I answered.

I reached into my pockets, only to realise my wallet was missing, as was my phone. I must have lost them both during the course of my bender.

“Shit!” I swore, drawing the barman’s attention. “I’m sorry pal, I don’t have any money.”

The barkeep shook his head and smiled, as he finished pouring my drink and placed the glass on the bar in front of me.

“Don’t worry my friend,” he said, “this one’s on the house. Then we can start a tab, depending on how long you decide to stay…Now relax, take a load off kid.”

“Okay, thank you.” I replied, whilst taking a stool and reaching for my glass. I still felt uncomfortable about this whole situation but thought I would take some time to get my head together, and perhaps see if the friendly barkeeper would front me the cash for a taxi home.

I was working my way up to asking him when the barman picked up a remote control and switched on the old television set above the bar. That’s when things got really weird.

He spent a few minutes flicking through the channels, showing a series of disturbing scenes – most of which were either hardcore pornography or acts of extreme violence. I’m not usually the squeamish type, but some of those images were truly sickening, depicting brutal murders and scenes of torture, all very graphic and realistic.

The barkeep paused his channel surfing for a moment, watching a sadistic talk show where two zombies tore chunks out of a guest, ripping his throat out with teeth and nails. And – as this violent murder played out – a studio audience cheered enthusiastically, as a smiling presenter addressed the camera.

Thankfully, the barkeep soon got bored of this grisly programme and he switched channels once again, ultimately settling on a soccer match. It all seemed fairly normal, that was until there was a close-up shot of the action, and I noticed the players were kicking around a severed human head instead of a ball.

“Jesus!” I swore, turning away from the TV in disgust as I reached for my glass and took a large gulp of hard liquor. I don’t know if it was due to the circumstances, but that drink was the best I’d ever had.

The barman kept the match on in the background but turned the sound down. There was no music playing in the bar, only the white noise – that irritating and slightly disturbing buzzing which seemed to be gradually growing louder and more intrusive.

It goes without saying that I was feeling pretty uncomfortable by this point. Not wishing to draw attention to myself, I scanned the barroom, anticipating that I might need to make a quick exit. I noticed then that there were no windows in the room, and the only lights were artificial. There was a door behind me, which I noticed was securely padlocked. This didn’t bode well.

I frantically looked to the rear of the room, noting a backdoor that appeared to be unlocked. I made a mental note of this while I continued to survey the room. The walls were adorned with disturbing pictures – photos of disasters and violent incidents, everything from the Hindenburg crashing in flames to Buddhist monks burning on the streets of Saigon.

These scenes of mayhem, destruction and tragedy only added to the menacing atmosphere. Furthermore, my fellow patrons continued to act very oddly, particularly the young man in the shell suit, who was still glaring angrily at me from the far side of the bar.

I shook my head, overwhelmed by the insanity of my surroundings as I reached for my drink, downing it in one.

“Care for another one?” asked the barkeeper.

I nodded my head in the affirmative, knowing I shouldn’t but somehow finding myself unable to refuse.

“What the hell kind of place is this?” I asked incredulously.

The barkeeper smiled ever so slightly before answering. “Well sir, technically speaking this is a shebeen…that is to say, we aren’t officially licensed by any earthly government or authority. But – despite our disadvantages – we try to offer a comfortable experience for our patrons before they move on to the next phase.”

“Okay.” I responded with a puzzled tone.

Frankly, his so-called explanation only raised more questions. ‘A comfortable experience’…in this shithole? Really? And what did he mean by ‘the next phase’? I was about to ask him when events interceded.

Suddenly, the man in the shell suit shot up from his chair, pointing accusingly at the barman and screaming with an intense rage.

“You’re a fucking idiot if you believe a word that bastard says!” he shouted, whist continuing to point at the barman. His dark eyes were full of hatred.

“Now sir,” the barkeeper replied calmly, “There really is no need for that kind of language.”

“Fuck you!” he shot back, “You’re the one who keeps us here! I going to make you pay, you fucker!”

In a flash, he lifted his beer bottle, smashing it against the table and creating a weapon from the jagged edges. Next, the shell suited thug cried out as he overturned the table and charged towards the bar, his improvised weapon in hand. I instinctively jumped up from my bar stool, preparing to defend myself from this crazed attacker, but the barman was way ahead of me.

I glanced across in time to see him pull a sawn-off shotgun from behind the bar, quickly aiming and firing at the attacker. The mighty blast from the gun reverberated throughout the room, as the buckshot tore into the attacker’s belly, throwing him backwards onto the filthy floor.

There was blood everywhere and the poor bastard’s guts were spilling out all over the ground. He screamed in shock and agony, the colour rapidly draining from his face as he tried in vain to shove his intestines back into his stomach. Meanwhile, the barkeeper re-aimed, firing again – this time blowing the man’s head clean off, splattering fragments of brain and skull all across the room.

“Jesus Christ!” I squealed.

“That guy never learns.” the barkeeper said calmly, as he emptied the spent cartridges from his shotgun.

“You just killed him!” I screamed in disbelief, while the barman simply shrugged his shoulders dismissively.

Suddenly, I heard laughter, and I turned my head to see the middle-aged woman smirking cruelly, seemingly taking a sadistic pleasure at the violent events which had played out before her. I shot her a disapproving look, but this only seemed to add to her amusement, as her mocking laughter grew ever louder.

The courting couple at the bar had remained oblivious to the shooting, before suddenly the gangster shot up from his chair, angrily remonstrating with his lover.

“What the hell!” he shouted, “I’ll kill you, bitch!”

He threw a punch, but the young woman reacted with astonishing speed and strength, grabbing his extended arm and snapping it like a twig. He howled out in pain, as she threw his body against the bar. To my horror, the young woman transformed in an instant, her formerly delicate features and kind eyes replaced by something primal.
I watched on as fangs emerged from her mouth and she bit deep into the helpless man’s throat, ripping out his jugular and spraying dark blood all over the bar. She continued to feed as the man’s body convulsed, the life slowly draining out of him.

I found myself in a state of total shock, unable to belief the sudden descent into bloody violence that I’d just witnessed.

“What the fuck is wrong with you people?” I exclaimed, not really expecting an answer.

While all this was happening, the older woman continued to laugh, her cruel cackle growing louder. But there was another sound assaulting my ears. The white noise…that damn buzzing. I felt like it was inside of my skull, so intense and overwhelming that I could barely think.

I knew I had to get out of there – I’d die if I didn’t. Fighting through the pain, I darted towards the rear of the bar, knowing it was my only way out. But the woman blocked my path, having finished feeding off her lover, his fresh blood dripping from her fangs as her eyes turned red, like those of a demon.

I ducked down and charged, somehow managing to avoid her grasp as I sprinted for the exit. I nearly slipped on the blood spread all across the floor, but thankfully I managed to stay on my feet, leaping over the mutilated corpse of the shell suit guy and never taking my eye off the back exit.

I slammed through the door, emerging in what I would describe as an enclosed courtyard. Immediately I found it difficult to breath, the air was so heavy and stifling. I panted as I looked up above me and was horrified to see the sky was colored blood red, like I’d suddenly stepped out onto the surface of Mars.

The courtyard had four high brick walls on each side, and the only visible exit was a heavy iron gate at the far end. But the path there was by no means clear. Guarding it was a huge figure, easily 7, if not 8 foot tall, dressed in dark robes with his face entirely covered by a hood.

This hellish entity stood tall, his head bowed as he guarded the gate and barred my way. He was truly terrifying and couldn’t possibly be human, but what really scared me was the beast he held on a heavy chain leash. A dog is what one might call it, but the beast was the size of a dire wolf, its eyes burning a demonic red and its snout filled with razor-sharp teeth.

The beast growled and pulled on its leash. Its hungry, hateful eyes were fixed upon me, and I had no doubt it would tear me to shreds if released. I was frozen to the spot, paralyzed with terror as I struggled to breath in the dense air. In my panicked state of mind, I considered fleeing back to the bar…Because, as bad as it was in there, I reckoned I had a better chance of survival.

But I knew I would never make it. If this hooded psycho released his hound, the beast would be ripping me apart in mere seconds.

I fell down to my knees, gasping for air as my eyes pleaded for mercy. The robed figure lifted his head ever so slightly, thankfully not revealing whatever horrors he hid underneath his hood. He loosened his grip and the dog barked so viciously. I feared the worst, but then he lifted his other arm, pointing at me with his long bony finger as he spoke in a deep and inhuman voice.

And what he said was…”IT IS NOT YOUR TIME. GO BACK.”

I didn’t need to be told twice, rapidly retreating as I sprinted back towards the bar’s back entrance, breaking through and slamming the door shut behind me.
It took me a moment to regain my breath and some degree of composure before I surveyed the scene before me. I expected to witness the same bloodbath I’d left behind, but to my astonishment, everything was back to normal, or at least as normal as things could be in a place like this.

The floors were no longer covered with blood and viscera, and the victims of the extreme violence I’d witnessed were alive and well, with no apparent injuries. The shell suit guy was sitting in the same spot as before he got shot, glaring back across the bar, and the 20’s gangster was back to flirting with the same woman who’d ripped his throat out, both acting as if nothing had happened.

The older woman in furs was no longer in fits of laughter, and the barman had laid down his shotgun, instead clicking through the TV channels, eventually settling on the same soccer match played with a severed head instead of a ball. None of it made any sense, but then nothing had on this crazy night.

It took me a moment to realise, but there was something different about the room. The front door – formerly closed and padlocked – now lay open, revealing a stairway leading upwards, and a ray of sunlight shining down, illuminating the otherwise grim and dark barroom.

I should have headed straight for the newly revealed exit, but I wasn’t quite ready to leave yet, so I called out to the barman to get his attention.

“Evening sir, what’s your poison?” he replied cheerfully,

I scratched my head in bewilderment, feeling a distinct sense of deja vu.

“Don’t you remember me?” I asked incredulously.

“Of course I do sir.” he replied almost defensively, “You came in for two drinks, went out the back, and then you came back in.”

I shook my head in frustration, taking a seat upon the stool and looking the barkeep straight in the eye.

“I don’t want a drink.” I stated firmly, “but I do want some answers.”

“Of course you do.” he exclaimed with a smirk, “Shoot away, my friend.”

I had my chance now, but the words stuck in my throat. Did I really want to find out the truth? Part of me said no, but a bigger part of me needed to know.

The first question was the hardest to ask, but I forced the words out of my mouth – “Am I dead?”

The barkeeper nodded his head, as if he’d anticipated this very question.

“Technically yes,” he replied coyly, “but, once you walk up those stairs, you should be okay.”

He pointed towards the staircase leading up towards the light. But I wasn’t ready to go yet.

My next question was a natural follow up. “This place…is it hell’s waiting room, or some shit like that?”

“Maybe…I don’t really know to be honest.” he replied, “People come in here, I serve them drinks, and they go out the back. Most never return. I don’t know exactly where they end up…heaven, hell, purgatory? Who knows? Not really my department. I just try to keep people comfortable and calm for as long as they’re here, and some stay longer than others.”

He motioned to the motley crew of patrons spread across the barroom. “And every now and again, they send someone back through. People like you, who aren’t quite ready to move on…”

My head was pounding, and I still didn’t understand. “But who are you?” I demanded, “What is your purpose?”

I saw his eyes light up, and his grin grew wider. “I’m the barkeeper…nothing more, nothing less…Look buddy, I get it. You want me to give you all the answers, but unfortunately, I can’t. Folks come through here, and I do my best to guide them.”

He raised his hand, motioning to the rest of the barroom. “I know this place is a shithole. There’s nothing I can do about that regrettably, but I do my best for my customers. That’s my job, and this is where my responsibility ends. I don’t mess with that scary son-of-a-bitch out the back. Couldn’t, even if I wanted to.”

I was literally flabbergasted. To have come so far, only to receive no real explanation. What was the point of it all?

There was only one question left in my head, which I asked through clenched teeth. “How can I avoid coming back here?”

The barkeep shrugged his shoulders. “Honestly, I don’t think you can. In the end, they all pass through here. My advice – for what it’s worth – is to make the most of the time you have, because life is chaotic and rarely fair, so you’ve got to squeeze whatever happiness you can from the whole mess… Now my friend, I think its time you headed home. Until we meet again?”

I felt like screaming out and thrashing the whole bar. What he’d told me was totally insane… But deep down, I knew the barkeeper spoke the truth. It was now or never. I had to leave this place or become trapped here forever. I took one last look at the barman and his ghostly patrons before turning my back and walking towards the door, ascending the staircase until the bright lights overwhelmed me.

The rest of my tale is probably all too familiar. I awoke in hospital, brought back to life by medical professionals after being clinically dead for several minutes. I ultimately made a full recovery and walked out of there to continue my life.

I won’t bore you with the details of what happened to me afterwards. Frankly, it doesn’t really matter. I know I’ll end up back in that damn place no matter what I do now or in the future, and honestly, this terrifies me. One thing brings me comfort however – the barkeeper and his dedication to duty, as he serves up drinks to the damned, doing so with a smile.

So, even though I dread the day of my inevitable demise, I can at least look forward to savoring one last drink, served up by one hell of a good bartender. Until we meet again, old friend.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "8a8e339a-8c68-4bad-9b29-c926b6aca412",
                    ThreadTypeId = 1,
                    Title = "Crimson Eyes",
                    Content = @"
It’s been an hour now and she hasn’t found a thing. She’s heard so many rumors about this old factory and all the things that have happened here since it closed down. She’s still intrigued about why it closed, or more specifically, the lack of reason. Apparently one day the owner just shut it down abruptly and laid off all his employees. And then, he just vanished. Never to be seen or heard from again. Then there’s all the stories about the “meetings” that took place here in the years after. She really thought that this would be a good place to catch some evidence, or just something in general. For as long as she could remember, she wanted to be a paranormal investigator and get some form of undeniable proof that ghosts and cryptids exist. She’s never been too scared of anything and tends to face most of her challenges head on. She prided herself on her bravery and courage, especially since her parents praised that trait of her’s as well. Nothing has ever stopped her before, and she wasn’t about to start letting it happen today. Most of her peers make fun of her for believing this stuff and started calling her “Carrie” even though that wasn’t her real name, but they seemed to think it fit her better. It used to bother her but then one day she just though to herself “screw it” and started to embrace it.

‘Drip’

“Just another puddle,” she thought to herself. The lack of activity was beginning to make her frustration set in as she started to get annoyed. She would be happy with even just one disembodied voice so she could show her peers and shut them the hell up for once. She looked up at the holes in the ceiling where she could see small rays of light coming from the tired sun as it slowly sinks lower than the horizon. Today it was a little darker than it should be at that moment due to some clouds that were rolling into the sky. “Must be a storm coming,” she murmured. She knows that rain and stormy weather can “charge” spirits which made her a bit excited but also added fuel to her frustration. “Just give me something you piece of shit!” she yelled into the empty air. After a few more minutes of disappointment, she was about ready to call it quits. She began to go through the room to collect her equipment that she had been using. She got to her still camera and was about to shut it down and pack it away when she froze. Out of nowhere she felt a chill run through her entire body, and she could have sworn that she saw a figure in the old manager’s office. Her adrenaline instantly began to pump through her veins and before she could even think about it, her body was moving towards the office on its own. The next thing she realized she was racing up the steel stairs to the manager’s office with the clangs of her feet stomping against the metal filling the empty building. She can see the office door getting closer and closer as her excitement grows and her heart beating like the double bass drums from her favorite band. She couldn’t believe that she was about to finally get the evidence she’s been dying for. She knew it had to be paranormal because there was really only one way to get into this place and she would have noticed it if someone tried to sneak by her. “Finally!” she almost yelled out as she burst through the office door. But as she calmed down, she realized there was nothing there, not even a gust of wind. Her eyes furiously scanned the area hoping to spot a figure that shouldn’t be there or catch something moving with no explanation, but nothing was there to be found.

Suddenly, her frustration peaked as she blurted, “You’ve gotta be fucking kidding me! I’m never gonna hear the end of this if I don’t get back with something!” Her anger kept swelling as she clenched her fists. Before she can act out further on this emotion, however, the door behind her slammed shut sending echoes throughout the factory. Nervousness crept in as she stood there startled. “This isn’t a door that just shuts on its own,” she whispered as she turned around to observe it. She also noticed that the whole time she was up there that there hasn’t been any wind at all to cause the door to shut like that. This made her even more on edge. Normally this kind of thing excites her, she has experience with past encounters, but this felt different. She felt darkness and something else, something sinister. In her body she felt what seemed like hatred, or maybe even malice. “Is there evil here?” she nervously wondered as she turned back around to search the office. But before she could process any more thoughts, she felt every inch of her body tighten up and noticed that this darkness started to grow from the far corner and began to fill the room. The sun is still somewhat up and isn’t covered by the clouds yet as evidenced by the light beaming through the holes in the ceiling, so she can’t understand why it’s possible for the room to get this dark.

Eventually the darkness engulfed the entire office defying the sun and practically blinding her. She stood there on the verge of tears, unable to move anything no matter how hard she tried. It was then that she saw it. “What the fu-,” she couldn’t even finish the thought. Before her eyes materialized something that terrified her down to her core. There was a pitch-black figure that stood in front of her. It definitely didn’t look entirely human, and it gave off such a horrible feeling. Every cell in her body wanted to leave immediately. This thing looked like it was made of pure darkness, just like the dark that filled the room. Yet for some reason she could see the figure perfectly, as if it wanted to be seen. It looked like it had long hair, maybe a little more than shoulder length, but each strand seemed to have a mind of its own as they managed to flow in ways that defied gravity. Its legs were disturbingly long, and its arms were almost as long but seemed to have claws instead of fingers. Just then, where its eyes should be, appeared two little orange lights that seemed to glow brighter than the sun, and she heard it chuckle.

“YOU MADE A MISTAKE,” it said mockingly. She looked at this thing with tears now beginning to swell in her eyes and she couldn’t find the courage to speak. This thing just spoke to her clear as day, but its mouth never moved. A tear fell down her face as its eyes turned crimson red and it felt as if the whole building began to shake. The whole place felt like it was going to crash around her, and it started to seem like she might actually die here. The figure became more menacing with each passing second with its bright crimson eyes looking right at her with no intention to ever stop. It started to feel like it was even looking into her mind. She was finally able to open her mouth to release a deafening scream as the figure laughed manically. Suddenly, the girl got the feeling back in her body again and didn’t hesitate a single second to start running out of there. She didn’t even grab the rest of her equipment because all she could think about was getting home to where her father and sister were. Once out of the factory she ran through the streets as the sun finally set behind her bringing the darkness with it. The air burned her lungs as she ran, pushing her body way past its limit as she was desperate to get home. Every time she closed her eyes she kept seeing those bright crimson eyes that belonged to the figure of darkness. She wouldn’t let herself stop running, she couldn’t. If she stopped it would get her, it felt like it was right behind her. She couldn’t tell if it really was behind her or if it infected her mind somehow. All she knew was that she couldn’t take it anymore and wanted to home, safe. Then, out of the corner of her eyes she saw the misshapen streetlight that marked the beginning of her street, bent slightly from when somebody accidentally hit it a few years ago with their car.

“Just a little further,” she thought as her body was ready to collapse at any moment. A rush of relief flowed through her body as her house came into sight and she darted up her porch, too terrified to even look behind her. After slamming and locking the door she fell flat on her back as her body finally gave in. She knew she wouldn’t make it to her room so she decided to just let herself fall asleep right there and then she would go back to collect her stuff tomorrow when the sun is fully out. As she lay there finally catching her breath, she began to smile as she felt her eyes close, she finally felt safe and comfortable. Right before she could fall asleep, she noticed something: it was way too quiet in her house right now. With the exception of her mom being on a business trip, her dad’s car was in the driveway and her sister really wasn’t the type to go out much at all especially at night, but it seemed as if she was the only person in the house. She began to get a little nervous as she got up off the ground and started to look for her family. “Hello?” she called out hesitantly as she quietly maneuvered through her home. She checked the living room, then the bedrooms, but was still unable to find anybody. There was no note, no text, and nobody tried to call her. This was very unusual, and it made her start to feel vulnerable. As strange as it was though, she decided she was way too tired to deal with this right now and just decided to dismiss it and head for bed. She could figure it all out tomorrow after resting. Giving up, she decided to get a drink of water before heading to her room. She went to the kitchen and grabbed a glass to fill. After finishing and feeling slightly more relaxed, she put the glass in the sink and turned to head for her room. Right as she turned the corner for the hallway, with her bedroom in sight, she froze on the spot unwillingly. A bead of sweat formed on her forehead as she struggled to move. Her heart started pounding against the inside of her chest and her mouth trembled as she used all her remaining strength to fight this. All she wanted to do was go to sleep and forget this horrible night. She didn’t know how much more she could take.

Suddenly, everything around her began to melt away. The walls, furniture, framed pictures, and the house itself began to literally melt. She couldn’t understand what was happening as her eyes raced back and forth trying to comprehend. Exhausted and unable to handle much more of this, the girl started to think that she was going insane as her blood pressure rocketed up and she started to hyperventilate. But that’s when she heard it. She began to hear that manic laughter that made her bones freeze over. It filled the air and invaded her ears causing pools of tears to form in her eyes. It wasn’t long before she finally realized what was happening. As her house melted away even further, it revealed where she really was the whole time. Where her warm comforting walls were now appeared to be rusted metal and cement. She began to see broken windows and holes began growing in the structure. Her tears began to stream down her face as she heard the laughter get louder and more sinister. That’s when she saw it. Stepping into the middle of the room from the corner was the dark figure that she ran from before. It was obvious to her now; she didn’t understand how but she never left the abandoned factory this whole time. She saw the crimson eyes that she was just trying to run away from, and they stared right at her. It seemed as if it was looking into her very core. Suddenly, the figure raised its arm, pointing at her. Out of nowhere, the girl found her body beginning to move towards it. But it wasn’t like she was walking towards it; it was more like someone was forcefully pulling her like someone was pulling a statue with a rope. Her feet dragged across the floor as the air got colder and the room got even darker. She was only a couple of feet away from the figure now and its eyes began to glow even brighter than before somehow as if it was getting excited, and her face now had rivers rushing down it. She was now but a foot away from the figure and she saw something that finally made her snap, the face of the dark figure seemed to start ripping horizontally about a little more than halfway down, seemingly revealing a disfigured mouth turning into a monstrous smile. The darkness around her, which already seemed as pitch-black as possible, got even darker. The dark figure now became indistinguishable from the darkness around it except for its bright crimson eyes, staring right back into hers. Her mind started filling with disturbing thoughts, and images of her family dying horribly over and over again. With the figure still laughing, the girl let out a terrible scream that was filled with fear, regret, and hopelessness. Suddenly, the crimson eyes vanished, and the girl’s scream just stopped abruptly. The darkness quickly vanished just as fast as it appeared, revealing the empty office in the abandoned factory with the moonlight now shining in as nothing happened. After a month of searching, the only thing that could be found was her abandoned equipment.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "331900b4-2c7e-4682-b0b8-5c5928eec238",
                    ThreadTypeId = 1,
                    Title = "Notes In The Dark",
                    Content = @"
Monday 28 December 18:00

I’m not sure how long humans are supposed to stay sane without human interaction but I’ve been doing alright so far I think. The silence is much worse anyway.

The initial shock of it all was extremely unnerving though. After the panic attack and near mental breakdown I sat down determinedly, pen in hand and started writing down what I knew about this whole situation.

Observation 01: Almost all living organisms in this city have vanished overnight.

People, animals and insects are all gone. I’m not sure about microscopic organisms yet. The only other living things left behind are trees, grass and most other plant life. I’m not sure how flowers and such will survive without other living organisms aiding them in pollination and such. I’ll watch and see.

Observation 02: Everything else seems to be as everyone left it.

Cars are still in their driveways and some doors to houses are open as if someone just stepped through the doorway.

Observation 03: Electricity is not running, phone lines and the internet are not working.

Street lamps are dead, phone lines just beep and browsers greet me with dead webpages. None of my contacts are responding and my phone won’t charge. It’s practically useless now.

Observation 04: The sun did not rise this morning.

Does this even count as a morning? It’s been at least nine hours since I’ve woken up and it’s remained pitch dark outside. The sky is still completely covered with clouds so no stars and moon can be seen.

Observation 05: The wind is gone.

I haven’t felt a breeze kiss my face or heard the rustling of a gust through branches since I woke up. The weather is showing no signs of changing.

Observation 06: There is no sound.

No birds or crickets chirping, no engines rumbling, nothing. The only sounds are the ones made by me. It all sounds much louder than it should. My footsteps on gravel, my breathing, the sound of this pen scratching on this paper, ringing in my ears; and the drumming of my own heartbeat. I can feel it in my head.

It all seems so wrong. I want to shrivel up inside myself and just disappear. The only thing keeping me sane is this watch. Thank God that it’s digital. I would lose my mind from analogue ticking.

And I can never forget my trusty flashlight making this all possible. I would be stumbling in the dark if not for it. I’ll look for some batteries later, I can’t risk it dying.

I think I’ll spend the rest of this ‘night’ gathering my things so I can explore outside the city limits tomorrow. I don’t know if this is some sick prank, emergency evacuation, or mass alien abduction but I will get to the bottom of this.
_____________________________

Tuesday 29 December 07:33

Last night I went to bed at ten-ish. Or tried to anyway. My heartbeat was unbearably loud and I was hyper aware of my bodily functions. The sound of swallowing saliva, my breathing, snoring, and worst of all my heartbeat. The constant drumming in my head.

All my things are packed. I filled my car with various foodstuffs from the local supermarket as well as batteries; lots of batteries, some portable lamps and more flashlights. I don’t think that counts as stealing.

Normally my conscience would bother me but right now I’m actually feeling good about this, excited even. It feels good being prepared. I have this all planned out. I’ve got some spare tires in the back, some jumper cables and first aid. And I could always hop into someone else’s car if need be. That dread of yesterday seems to be gone for now.

The nearest town is about 58km from here. If I drive at a safe speed of 30 km/h I should arrive in two hours, more or less.
_____________________________

19:00

So it turned out that this town is empty as well. No light, no sound, no wind; nada. The sky hasn’t cleared yet either. I’m starting to wonder if that’s even cloud cover up there. It’s impossible to make out. Anyways I found a place to stay for the night, a little three bedroom house.

My heart sank as I walked by their empty dog kennel. It reminded me of Lady. Waking up yesterday without her sleeping at my feet was bad enough. But when she didn’t respond to my calls it filled me with dread. It wasn’t long before I realised she was truly gone. Her and everyone else. It broke my heart to leave home.

All I have left of her are my memories and her squeaky bone. I’ll keep it in my pocket from now on.

I spent some time searching this place. Not really for supplies but to learn more about the family. The framed photos around the house lead me to believe that a family of three once lived here. Brings back memories. I should have spent more time with them.

On this very desk sits a small photo of them together, all three dressed in white shirts and grinning at the camera. It made me smile. It’s not human interaction but it’s better than nothing. I put the photo in my pocket.

Earlier I went out looking for a loudspeaker. It’s no use searching every single dark house so I drove through the town and called out for people through the loudspeaker. I should have worn earplugs or something, my ears were not prepared for the sudden noise. Now my ears are ringing even louder than before and It was all for nothing, there were no signs of life.

During my search of this bedroom I found a handgun under the bed. I haven’t identified any danger out there but it’s better being prepared anyway. I don’t know much about guns but I know enough to be able shoot and load one.

My dad showed me once when I was younger. Funny how the memories flood back to you after you lose the people you shared them with. The gun is fully loaded. I hope I won’t have to use it.
______________________________

Wednesday 30 December 09:26

I’m no stranger to sleep paralysis. It’s creepy waking up in the dead of night with a paralyzed body, what was always worse for me though is the shadow man who would watch me as I lay there helplessly.

It’s a sleep phenomenon. The shadow men aren’t really there, it’s all in your head and generated by your subconscious mind while you’re asleep. I learned that the best thing to do is stay calm and keep my breathing even, while reminding myself that the man can’t hurt me.

I was still quite young when I last had sleep paralysis and at that time I didn’t know all this about sleep and that it was all normal. My parents would wake up to little me screeching in the dead of night and rush into my bedroom expecting to find a burglar or at least an actual threat.

But they would always find me shaking like a leaf beneath the covers and bawling about the shadow man watching me.

They would try to calm me down, stroking my forehead and reciting Psalm 23:4. They told me it was all in my head, that none of it was real. It was real. I would scream back at them. I would tell them I saw him with my own eyes and that he stood right there at the foot of my bed.

They were worried so they did some research to prove to me it was all a natural phenomenon. They took me to a nice grey haired doctor explained it to me one day with some cartoons and infographics I could understand. He told me it was perfectly normal and happened to other people as well.

He said that we all have sleep paralysis every night to stop out bodies from thrashing around and getting hurt during dreams; and that the shadow man was nothing but a figment of my imagination. He gave me a red lollipop and sent me on my way.

My parents got me some new lava lamps and that was it. I had a few more experiences with the phenomenon but after a few months it stopped. I can’t remember if little me bought the doctor’s explanation but I remember just being glad when the sleep paralysis stopped soon after.

The thing about having sleep paralysis in a dark new world, is that it’s hard to tell dreams from reality. It’s so dark that when I lie down I can’t even tell if my eyes are open or not, whether it’s all a dream or not. Last night I had a reunion with my old friend. He was even darker than this eternal night.
_____________________________

11:59

I’m aware that this food will expire sooner or later so I’ve been eating only fresh foods like fruit and vegetables, while stocking up on canned goods and honey which will last longer.

I haven’t found any vegetable gardens or fruit trees yet but I’m interested to see If fruit and vegetables are still able to grow. It shouldn’t be possible without sunlight and rain right?

As long as I can find a grocery store I’ll be good I think. I’ve heard that honey doesn’t expire at all. Let’s hope so.
______________________________

20:00

I’m not sure what to do next so I’ve been sitting around and thinking about the old days. It’s painful to think about it all. All I wanted was to be alone and now I finally got what I wanted, so why am I crying?

I’ve been staring at this photograph for an hour. These strangers I’ve never met, they make me feel more human.

I miss the simple things in life. Swimming on hot summer days, stargazing, watching the sun set. It hasn’t even been that long and it already feels like I’m losing it.

This is so messed up. It feels so wrong. I can feel every cell in my body protesting against this new world. Humans aren’t made to deal with life like this. This isn’t life. Is this the afterlife? My parents taught me that hell is a lake of fire, so even this can’t be it. If this is heaven I would rather die.
______________________________

21:00

I’ve spent some time walking around outside. The darkness is like a heavy and oppressive blanket. My shoulders hunch over as I walk. I feel like I’m carrying the world on my shoulders.

If I were claustrophobic I would have died a long time ago. The air is thick. It’s hard to breathe

The silence is mocking me. Sometimes the silence is unbearable and sometimes the ringing in my ears is deafening. I’ve been starting to click my teeth together habitually to create some sound and drown out the ringing. And the heartbeat. I can feel my heartbeat in my head. I feel like God’s plaything.
______________________________

23:00

I’ve been sitting in my car with the engine on. The rumbling is comforting.
______________________________

Thursday 31 December 05:02

I think I know what to do now. I’m quite certain that this darkness must be affecting the whole country and maybe even the world. I think the next step is to find out for sure.

Even if this darkness won’t end I need to find out the reason why. I need to know if the sun and moon are still up there at least. I need to get to the beach and see if there are waves. If so then there is still hope.

If not, this world has changed forever and chances are I’ll never see the sun and moon again.

The ocean is 1,900km from here. With rest stops for sleep the entire trip will take over 80 hours. I need to drive slowly and cautiously, I can’t risk missing something important on the way or crashing on the long road. It will be a challenge but I need to keep moving.

The preparations are complete and I’m ready to leave.

I need to find people. I’ll keep the photograph on the inside of my windshield. To remember what I’m fighting for.
______________________________

10:22

I’ve been driving with the car’s interiors lights on, as well as the MP3 player which I completely forgot about.

I’ve been playing Mister Sandman on loop for the past few hours. It reminds me of home and of my mother. She would always sing it to me at night to calm me down after my late night panic attacks. They were so good to me. I didn’t deserve them.

I can’t turn this music off, it’s yet another thing keeping me going. And Lord knows I need all the motivation I can get.
______________________________

17:00

I hold the squeaky toy as often as I can. Lady was a good dog. It was us two versus the world. She was always sweet and gentle. She would bite this bone softly, just enough to make it squeak.

Evident by the very few bite marks on the bone. There are three scratches on the bone to be exact. Now I know it like the back of my hand.

This bone, the CD in the player, and this photograph are my symbols of hope. They keep me going. I’ll hold on to them as long as I can.
______________________________

Friday 1 January 09:00

My friend, the shadow man, decided to visit me again last night. I saw him in my rear view mirror, he sat watching me on the back seat.

The light of the lamp seemed to curve around him, evading him. He disappeared after a moment and I was left unsure of how to feel.

At least I’m not a scared little kid anymore.
______________________________

15:00

The drive has been uneventful. I’ve made a few stops along the way to use a toilet and restock on fresh foods.

Other than that my mind has been cloudy. I feel like I’m half asleep, I really shouldn’t be driving with this state of mind. But I’m running on fumes, I don’t want to lose my momentum. I feel like my sanity is draining away with every passing moment.

I need to keep moving, if I stop now I may not be able to start again.
______________________________

Saturday 2 January 01:02

My worst fear has been realized. My car broke down. I’m such an idiot. I should have saved the battery. I’ve been keeping the engine on to help me fall asleep. I needed that MP3 player on.

I’ll admit it, I’m scared. And staying sane should be my own top priority right? I have food and water; this trip is just a side quest right?

It doesn’t matter. I’ll walk. If I stay in this car I’ll eventually starve to death or lose my mind. I need to keep moving.
______________________________

06:00

I’ve been walking non stop for hours. I was so afraid to stop. But my body is about to give up. All I took with me is my backpack filled with food and water. I realized a minute ago that I forgot the photograph behind in the car.
______________________________

06:50

I found a car on the road. A white Volkswagen CITI Golf. It brings back memories. It was the first car I had ever stolen and hotwired. The memories are flooding back, I was so young.

I had everything a child would ever wish for. A loving family, the newest toys, love and attention. They were so good to me, and I traded them off for cheap thrills and delinquency.

I’ve been able to start the car, I’ve never been able to forget the sound of this engine.
______________________________

My watch is broken so I don’t know the time and I have no other way to separate these diary entries. I think it smashed against one of the rocks.

I crashed. I thought I saw someone on the road. I drove into a pond or something. All I could salvage was this flashlight, a pen, a pack of dried fruit, and this diary which somehow survived. The pages are wet but it’s still usable. Almost forgot the gun, I still have that. Saved by the belt.

I’m going to follow this road and see where it leads me. This road cuts through a mountain, they go so high that I can’t see the top. I don’t know how far the next town is but I’ll keep moving forward.

I’ve lost everything. I start out with everything and I lose it all. The reality of the situation is that I can’t blame this world for what I’ve lost. I lost it all before any of this even started, and I only have myself to blame.

It’s been a long while since my last entry. I can’t tell how long for sure but it feels like forever.

I’ve started seeing hallucinations. Abstract colours and shapes float around my vision.

At least my footsteps on the road have been drowning out the ringing. I’ve been trying hard not to stop but my feet hurt, at least it’s an opportunity to write an entry.

I woke up face down. I don’t know how long I’ve been laying here for. I can’t stop.

I’m gripping Lady’s squeaky bone tightly. I’m holding on to hope.

I’ve started hearing voices. I keep thinking Lady is following me not too far behind.

Same old road.

I keep thinking of home.

I miss the bible stories they would tell me. I was such an ungrateful child.

I’m breathless but I keep singing Mister Sandman. The sky feels heavy.

I just noticed I have a deep gash in my right calf. It must have been a sharp rock. Explains the numbness in my leg.

I’m so tired.

I feel like I’m locked in a dark, musty closet.

I keep getting the urge to drop my flashlight but I know I can’t.

The hallucinations are getting worse. I’m starting to lose it.

I keep drifting into the past. Actually it feels more like the past is drifting around me.

I keep forgetting I’m not seven years old anymore.

I keep waking up face first in the dirt. My lips are swollen and bleeding.

It’s hard to eat this fruit.

I told you I’m not a kid anymore.

It’s hard singing with swollen lips.

Time is not real.

I keep forgetting who I am.

I don’t know where I was headed, but straight seems like the right choice.

How on Earth am I still alive?

I’ve passed out so many times already. I want to lay down and die. The sky is dead.

The walls are closing in, the mountain wants to eat me.

The air is so thick.

My cuticles are bleeding from gripping this diary. Why am I carrying a squeaky bone?

Time is not real.

What happened to the sun? My shoes are messed up.

It’s hard to breathe.

It’s my first day of school today!

I’m so tired. But I need to keep walking. And writing when I’m not. How many days has it been?

What are those voices? I feel like I’ve been walking forever.

I called out for Lady but she isn’t coming. I think my lips are swollen. I am so confused.

Who is Lady?

I threw away the squeaky bone. I don’t even know why I have it.

I feel like I lost a piece of myself and I don’t know why.

Mom makes the best hot chocolate.

Time is not real.

I think someone is following me.

I can see Dad working on his car. He taught me all I know.

I stole my first car today, a CITI Golf. I think I’m in.

I told you all I’m not a mama’s boy. I’m the best in this crew. It feels good having brothers. What would Steven think?

It must be the hardest thing in the world for them. Having to explain to their son that his twin brother died in a car accident.

That’s all in the past now. I have other brothers. And they need to be taken care of.

Seven cars, I’m on a roll.

If only I protected Steven.

I can’t bear that look of disappointment on their faces.

I can’t be near them.

They’ve been so good to me. I blamed them to their faces but deep down I always blamed myself.

I can’t be near them.

Mister Sandman, bring me a dream.

How long have I been walking for?

My stomach is aching for food.

How did I get here?

Every muscle in my body hurts.

I’m holding on to hope.

Why am I still walking?

When did I learn to walk?

The doctor gave me a cherry flavored lollipop. I don’t buy his explanation.

I woke up screaming.

I abandoned my parents when they needed me the most. I should have visited them. I should have said sorry. But I was ashamed. I wasn’t worthy of their love and forgiveness. I can see Steven in front of me right now.

This is the end of the road. I can’t walk straight anymore. I can’t even walk. The shadow man stands before me and I’m not afraid. He’s been waiting all this time to welcome me home.

The parents from the photograph stand on either side of him, smiling their toothy smiles. I hear a dog barking. Mister Sandman plays from someplace distant.

The air is vibrating and the couple’s faces have changed into my parents’. They are both chanting the fourth verse of Psalm 23. Mr Sandman is playing louder now.

The shadow man is gone now. I see Steven standing between them. He’s smiling. I should join them. They’ve all been waiting all this time. And I’ve been stubborn as usual and kept them waiting. This song keeps getting louder.

That’s where all the trouble started. My stubborn nature. It’s time to give in. It’s time to repent. The air is shaking. The ground is moving. The chanting is in my head. The fear is gone. They are waiting with open arms. The song is playing in my head. I feel like my head will explode.

I don’t know if this gun can still fire. But I’m going to try anyway. I’m going to join them. I need to leave this dark place. I’ve heard when you go to heaven you see a bright light. I’m not sure if I’ll make it in, I don’t think I’m worthy. But even the flames of hell will produce some light. I’m ready. If this is the last entry, the gun fired.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "6aa06fee-025d-4120-aa4e-9b6310273443",
                    ThreadTypeId = 1,
                    Title = "Room With No Windows",
                    Content = @"
As his aged, wrinkled fingers weaved through the long beard flowing from his face, that noise came again. Only this time, it was louder.

There he sat, hunched over the stained oak table that had arrived years ago with no explanation. It had come whilst he slept; absent one night, and there the following morning. Probably from the people above, he deduced. That’s where he got everything. In this room with no windows. The only room he’d ever known in his eighty-four years on this earth.

Again the sound reverberated off the four thick walls of his home. So much so that the candles scattered across the table shook with ease, their yellow candlelight dancing off the glazed old eyes of a man who had come to know them as friends through the passing years.

A sudden lurch thrust him from his seat and onto the dirt floor below. In a mad scramble, it dawned on him quickly that this room with no windows, his room, was moving. Objects that had remained idle for ages now found themselves scattered across the interior, and the candles themselves proved no match for the tumultuous forces that knocked them level.

He staggered to his feet. Instinctively, he felt for the table’s edge to support himself, but his hand met only pain in the form of hot wax. It scoured, but nothing could deprive his attention from the jostling of the room.

Another lurch, and he was again thrown to the ground. But unlike the first, this force kept him pinned, and he had the sense that he was moving up. The oak table quivered on its four legs before surrendering to this mysterious influence, and the last of the candles was extinguished. He found himself draped in darkness.

And then suddenly, it was dark no more. The ceiling was lifted off, and following it came a bright light, brighter than any he had ever seen before. The people from above, he thought to himself. As his eyes adjusted, the bright light became blue, and he could see that there were puffs of white moving briskly across this blue firmament.

And it became only stranger. Two men, who very much resembled him save for their younger and broader shoulders, reached down for him, each grabbing beneath the old man’s underarms. The people from above. They wore plastic, yellow hats and bright reflective vests. He found himself hoisted into the air and dropped on a wooden platform that stood adjacent to his room. Only now, there was far more to see than the mere confines of his quarters.

He appeared to be on a beach, or at least he thought he was. All his life he had been told of places such as these, and the word beach was the first that came to mind. In every direction, people of all ages surrounded the area, expressions of vast wonder and hysteria lining their faces. For the first time ever, he’d been granted a new perspective he’d never imagined in his wildest dreams. For the first time ever, he was able to see his room for what it truly was.

It was a box, and judging from the deep depression in the adjacent sand, it appeared to have been buried there for eons, not once seeing the light of day until its violent removal mere minutes ago. The crowd roared abruptly, encouraged by another individual who had leapt onto the wooden platform next to the old man. He was a middle-aged man wearing a pinstriped suit and black top hat. Like a deranged ringleader.

“Welcome one and all,” he bellowed, “to the world’s first ever human time capsule!”
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "651c10e9-6439-479e-8fad-c5784ecff65b",
                    ThreadTypeId = 1,
                    Title = "The Cat Killed Curiosity",
                    Content = @"
My years of school have finally come to an end. With that in mind, I wanted to enjoy myself as much as I could before coming to grips with the rigors of adult life. After all, I did bust my ass within those four years just to earn a worthless piece of paper that says “I did it”. Either way, it didn’t matter. I was going to graduate and move on with my life, so I suppose I’d treat myself with a nice quiet slice of relaxation.

But before I left, I purchased several books about the paranormal. Reading the accounts of those who’ve witnessed such phenomena had always stimulated my morbid curiosity of searching for untold apparitions. Upon finishing purchasing said books, I made my way to my lifelong friend’s house, Vinny LaMotta. Oh, I’m Tony, by the way. My surname is irrelevant.

“Aw, you didn’t make any coffee?” I asked politely.

He placed down his newspaper and gave me a stern look. “What do I look like, a fuckin’ caterer?” he answered scornfully before returning to his newspaper.

Vinny and I were ethnically Italians. However, Vinny was an undeniably devoted catholic. He was always a firm believer in religion, all else be damned. I, on the other hand, wasn’t much of a religious guy. In fact, I strongly considered converting to agnosticism due to my disbelief for anything remotely beyond human capabilities.

Vinny allowed me to stay at his place for as long as needed until I got myself admitted to whatever post-secondary institution I might find. Like I said, for the time being, I was going to enjoy my free time to its fullest before starting a new phase of life as an adult.

I made myself comfortable as much as Vinny’s household allowed, before delving into the plethora of literature I purchased at the library. According to this one book whose title I don’t recall, this so-called children’s game, famously known as the ouija board, was a possibility for ghostly ethereal beings to visit our world. To make matters more interesting, the book even listed out the rules before playing.

With that being said, I wouldn’t let my strong skepticism interfere with my curiosities. I was fascinated with these kinds of things so much so that I browsed the internet for people who’ve had such experiences.

Now, I know what you might be thinking: these things could’ve been easily fabricated to deceive viewers, given the technological capabilities that the modern world has to offer. I knew better than not to take these claims with a grain of salt.

Knowing Vinny and his possible zero-tolerance policy of discussions about ghosts or demons, I hadn’t opened up to him with that subject because he’d probably shoot that shit down in an instant, so I kept this piqued interest of mine to myself for now.

Days afterward, I could’ve sworn my boredom was borderline lethal. I’ve tried to cure myself from it many times, but even after reading and listening to every paranormal encounter, it grew stale. The only way I could hope for any satisfaction was to conduct these explorations myself. A light bulb dimly lit above my head: I’d call some of my old friends, and we’d make some arrangements to try contacting “the spirits”.

Much to my surprise, my friend Tommy had an old ouija board that was gifted to him. He said he wanted to celebrate a small get-together with drinks. My eagerness got the best of me, so me being me, I strongly accepted the invitation. As I write this, my hefty exhales are a reminder of my incredibly stupid and avoidable path.

I told Vinny I’d be back sometime around morning or so, as I’d be heading to a party. Vinny didn’t really care, as long as I didn’t cause any ruckus around here.
I wasted no time in getting ready. Once I was at Tommy’s place, he and my other two old pals were there, setting up the drinks for the establishment. The board was perfectly centered on the wooden table. It was beautiful: its crystal-clear surface was reflective with small sparkles near its edges; its letters and numbers were perfectly emplaced in their respective orders; and despite its old age, the board would have you think that it was exported from its manufacturer, just hours ago.

I met with the other guys down at Tommy’s basement. There was Tommy, the guy who helped me orchestrate this whole thing with booze and cigs; then there was CJ, better known as Crazy Johnny. We called him Crazy Johnny for a reason. And finally, there’s Franky, one of Johnny’s close friends.

We gathered around the table with a colorful variety of alcoholic beverages provided for.

Johnny and I helped ourselves with the provided refreshments. And by the time we had our glasses of whatever drinks were available, we clink our glasses altogether, celebrating our small reunion. To make this even better, we had decided that we’d bet wads of cash to anyone who could attract the most ghosts or demons.

We unsurprisingly disregarded several important rules when playing: don’t play alone, don’t provoke whatever you’re communicating with, and most importantly, don’t play at home.

But who was I to emphasize the significance of these asinine rules? After all, we were playing under the heavy influence of alcohol. “Disregard” was never a part of our vocabulary. We did what we wanted. Furthermore, I honestly thought that disregarding the “safety measures” of the game would increase our likelihood of getting into contact with anything, good or bad.

We had our fingers on the planchette. I was up first. I asked for any signs of spiritual presence. Awkwardness ensued. Nothing happened, except the others were liquoring up.

I asked for any signs of presence again, except I was the only player during the session since the other guys lost interest. Soon enough, the planchette slowly moved to “yes”. My eyes widened with curiosity at a staggering rate.

In spite of many paranormal encounters going downhill, I still pressed on. I wanted to prove to the world that such things were true, not just some fairytale bullshit.

“Who are you? Or better yet, what’s your name?” I asked sincerely. The others were still getting liquored up meanwhile I finally found..something. Seconds later, the thing spelled out its name: R-A-V-E-N. I was taken aback. I’m possibly communicating with a female spirit in our presence.

“Are you actually real?” I asked more, smiling with slight excitement from this rare occurrence. Raven responded with “YES”. That was the moment where I should’ve completely ceased communication with the ghost.

“Am I handsome?” Raven responded with “yes” once again. I gasped from excitement, laughing out loud from this awesome encounter. I informed the other three that I was in contact with Raven.

In his slightly inebriated state, Tommy commented, “you broke your cherry!” Laughter erupted across the table, as he piled a whopping six-hundred dollars next to me.
Tommy encouraged Johnny to join me. Knowing Johnny, he wanted to be part of the action. And so he was. The three of us stood around Johnny, waiting to see if he could attract Raven more than I did.

Johnny took a quick drag from his barely lit cigarette. With all his mighty confidence he mustered, he asked the ghost out.

“Hey honey, how ‘bout we get some coffee sometime, eh?” We stared at the board waiting for a response, with wads of cash just waiting to get deposited into Johnny’s pocket.

I shit you not, in a split second, Raven rejected Johnny’s humorous yet sardonic question. We all exploded in screeching laughter. The way the planchette quickly moved to the “no” was breathtakingly hilarious!

“Ooh! OH! Hahahaha! I can’t believe what I just heard!” Franky shouted, giggling with his tongue extended outwardly. “Ay, Raven, here. This is fuh’ you! Atta-boy! ” Franky added, as he joyfully placed a seemingly uncountable amount of cash next to the board.

“Ahahahaha!” we guffawed, our laughs gradually growing louder.

“I got a lotta respect for this ‘ghost’. She’s got a lotta balls, good for you! Don’t take no shit from nobody,” Franky cheered again. We hollered in laughter as Johnny’s face turned a tomato-red from absolute embarrassment.

“Can you believe this, he asks a ghost on a date and the ghost basically tells him to go fuck himself, hahahaha!” Franky laughed, continuously placing more money next to the board. Our laughter, along with Franky’s snarky comments, perfectly intertwined together.

Johnny just sat there, dumbfounded and embarrassed. His humiliation contributed to his sudden looks of sheer glower. He was awfully quiet, too.
“You gonna let her get away with that? You gonna let this fuckin’ punk get away with that? What’s the matta with you, what’s the world comin’ to?” Franky persisted yet again with his undeniably hilarious questions.

Immediately afterwards, Johnny did the unthinkable: he somehow drew a pistol and released a whole magazine of rounds at the board, turning the glass object into scattered smithereens.

“That’s what the fuckin’ world’s comin’ to, how you like that?” Johnny barked, with pure satisfaction. “How’s that, alright?” he added on once more.

Franky angrily disarmed the Crazy Johnny, criticizing him in fumes. “What’s the fuckin’ matta with you? WHAT is the FUCKIN’ MATTA WITH YOU? What, are you stupid or what? Johnny, Johnny, I’m kiddin’ with you. What the fuck are you doin’? What are you, fuckin’ nuts?”

“How do I know you’re kiddin’? You’re breakin’ my fuckin’ balls?!” Johnny yelled back, waving his hands dismissively, understating his unexpected reaction.

Franky barked back. . “I’m just kiddin’ with you! You fuckin’ shoot the board?!”

A brief silence immediately followed.

“Good shot. Whaddya want from me? I’m a good shot,” Johnny said casually, completely unenthused by what had just happened.

“How could you miss at this distance?” I interjected.

Franky angrily slammed his stack of cash on the table. “Stupid bastard, I can’t fuckin’ believe you. Now you’re gonna clean this damn mess up. You’re going to bless this place yourself, ‘cus I got no fuckin’ holy water. Johnny rolled his eyes and went along with Franky’s orders.

I went to Vinny’s. On my way there, I couldn’t help but laugh at Franky scolding Johnny for his exceptional reaction. Some part of me felt slightly paranoid because I also disregarded yet another important rule: always say goodbye. Of course, me being me, I regarded it as just another asinine rule that finds its way to ruin my fun.
I know, I know, I obviously have been grossly negligent with communicating with the unknown. Despite this, it was there that I knew I belonged. I knew for sure that investigating such things would be an excellent career path.

I told Vinny about the hilarious experiences that were had during the session. I’ve also told him about Raven, the seemingly benevolent spirit whom I’ve got in contact with.

Upon hearing this, Vinny’s eyes grew to a miniscule degree.

“I don’t want any more of that shit,” Vinny commented.

Assuming he meant something else entirely, I questioned further. “What shit? What are you talkin’ about?

“..You know what I mean. That. That garbage. Just stay away from it.”

“Vinny, wha-“ I attempted to ask before he cut me off.

“I’m not talkin’ about what you did there. You did what you had to do. I’m talking about now. Now, here, and now.”

I returned a puzzled look. “Vinny, why would I want to get into that?”

“Don’t make a jerk outta me. Just don’t do it! Just don’t do it. Listen, I ain’t gonna get fucked like my cousin Joey, you understand? He’s almost 30 years old, and the fuckin’ guy’s gonna get dragged to hell. I don’t need that. You understand?” he said with a voice of concern increasing ever so slightly.

“Uh-huh—“ I nodded.

“So I’m warning everybody. Could be my wife, my friends—could be anybody. Joey’s goin’ to hell just for saying hello to some fuck he summoned on that board. I don’t need that shit. Ain’t gonna happen to me, you understand?”

I nodded in agreement once again.

“You know that you’re only unaffected ‘cus those things haven’t attached themselves to you, and I used as much holy water to deter them from us. I don’t need this heat. You understand?”

“Yeah,” I said as I went along with Vinny’s repetitive warnings.

“You see anybody fuckin’ around with this shit, you’re gonna tell me, right?”

“Of course-” my agreement was abruptly cut by Vinny lightly slapping my face.

“That means anybody. Got it?” he concluded authoritatively, with that momentous gaze.

Honestly, I couldn’t be bothered to heed Vinny’s warnings. I did so just to spare myself from his antithetical viewpoints. As far as Vinny was concerned, I was going along with the program.

I felt pretty worn out despite not doing much, so I went back to my home. As I laid on bed, I couldn’t dismiss this overwhelming feeling of being watched, being paranoid, if you will.

I had to know more about Raven. Who she was, where she was from, and things of that nature. Since Vinny wasn’t around, I crafted my very own makeshift ouija board. As I did my usual thing by asking for any presence, I got a response. Much to my surprise, it was Raven herself.

I couldn’t fucking believe that I disregarded the fact that Raven had followed me from Tommy’s to my own home. Regardless, I inquired about the spirit’s past life with as much tact as I could muster, just to spare myself from any potentially negative consequences. She hadn’t responded.

The planchette was moving slowly but steadily, as it spelled out “free me”. I gave a puzzled look. Confused, I clarified with the ghost in question by asking her if that was a question to which she answered with “yes”.

I wasn’t sure how to respond to this, although this was absolutely the perfect time to take advantage of this rare opportunity, to broadcast to the world that these beings are completely real. With that in mind, I wasted no time setting up my phone to record the ongoing session.

I accepted Raven’s offer to free her. I asked her to confirm her presence by any means necessary to satisfy the YouTube audience. A few moments later, the makeshift board slowly levitated upward, and I just stood there with my jaw dropped from absolute surprise. In a split second, the board bursted into a fiery blaze and immediately dissipated into the thin air, with its remaining dust particles strewn across my house floor.

Upon witnessing this unbelievably surreal moment, I gasped from excitement and slight fear, ending the recording. At that time, I obviously wasn’t concerned in the slightest about the perils of the seemingly benevolent being, so I continued on with uploading the video.

The video gained some traction, although the majority of its ratings were substantially negative because of its incredibly doubtful nature. Some comments strictly warned me about being so negligent with the “potential release” of an unknown entity in my house, but I honestly didn’t really pay any mind to them nor did I take any of their concerns into consideration.

Soon enough, that same dreadful feeling of paranoia and weariness had struck me once more. It was becoming a real pain in the ass, so I did what any normal person would’ve done and tried to sleep it off.

Weeks later, the video’s view count exploded colossally! As I scrolled through the ratings and my subscriber count, I was thrilled with sheer excitement. “AH, HAHAHA! TOMMY, those sons of bitches!” I laughed aloud excitedly, driving my palm on my desk.

When I noticed that the exact view count was over nine-thousand, I cried out once more. “OOOoh, TOMMY!” I was like a kid. In my mind, it was like winning the Nobel prize for validating the theoretical existence of the paranormal.

Tommy would probably be the only person whom I know who’d give a shit about these abnormal findings. I’d have definitely paid him a visit if I hadn’t been so fucking stupid. Besides, he’s been outta town since, so I’d have to wait to tell him about my thrilling discovery.

Days later, the hype had died significantly, and I don’t know why, but I felt… vulnerable sleeping in my own room. Nobody else lived with me, as my parents had me emancipated years ago. I did get drunk as a small self-celebration, however, I woke up around approximately two-fifty nine AM.

I’m honestly surprised I managed to know that, considering I drank heavily that night. Then three AM hit. I was hungover, but even then, I was aware of my surroundings. The room was mostly pitch-black, aside from my TV’s brightness’ gleams piercing into the vast darkness.

Speaking of TVs, I don’t remember turning mine on at all. I took a closer look and realized that static was playing. I once more dismissed this as a possibility of one of my drunken antics that I can’t seem to remember, but something just…wasn’t right. I hadn’t bothered locating my remote controller, so I had decided to manually change the channel by the TV’s buttons.

All of a sudden, my TV vehemently erupted a deafening sound of distortion, combined with an indiscernible voice that I couldn’t decipher. I turned around and jumped back onto bed, covering my ears to minimize that ear-splitting noise. As it stopped, I turned around to look back at the TV again. The screen was now displaying what appeared to be a picture…of a black cat? It appeared to have its eyes closed.

I foolishly regarded this as one of my drunken antics again, and as I approached the TV to power it off, the damned cat opened its eyes, startling the ever-loving fuck out of me, causing me to jump inches back. That thing had hollow eyes with thick red glowing eyes for pupils. I gulped. I moved side to side, and the thing’s pupils were following my every move.

I know what you’re thinking: the red pupils and hollow eyes, the generically cliched creepypasta theme, sure. I would’ve probably regarded it as that, but I was alone in my own house. It was dark in here, and that thing was staring me down, so of course I was nervous.

I stood there motionless, staring at that..thing. My heart was furiously beating in rapid rates as I fell victim to its very whims. A myriad of questions overloaded my mind on just what the fuck was going on here. Before I could even ask what it was, the thing spoke.

“Raaaven,” it remarked in such a sinister tone that I can’t even describe, rotating its feline head clockwise. The disturbingly forced rotation of its spinal bones caused me to physically cringe.

“H-how are you even talkin’? How are yo-you even on my T-TV?” I trembled.

“I shall answer simply. I am capable of manifesting myself through digital means.” She bared a set of fangs as a means of intimidation. “Toni, you have voluntarily released me from His grasp. For that, I shall now claim you as mine,” it said once more menacingly, as her head was stuck in a tilt.

It was late at night and I was unquestionably paralyzed from sheer fright. I couldn’t fathom this thing’s sudden appearance in the slightest. My heart almost literally fell, as Raven fixed her tilted head back to its original position. God, that fucking horrible sound of the forced positioning was causing every inch of my neck hairs to stand up one by one.

Without question, I lunged myself towards my door, desperately gripping the knob with all the courage I could’ve mustered, only for the piece of shit to fail on me.

“I don’t think so,” she grinned, silently giggling to herself. I stared in horror at my door knob, as it wasn’t going to open whatsoever. Then my room lights began flickering as that damned thing was laughing hysterically at me.

“Fuck, fuck, FUCK!” I shrieked in absolute distress, physically pounding on the wooden door from my ever-increasing hopelessness.

I sat with my back leaning against the door, weeping in my arms as Raven grinned sinisterly. In a split second, the TV imploded, releasing a unit of sparks from the now-defunct vents. My room was now fully pitch-black, as the TV malfunctioned. Raven was gone for now.

Much to my surprise, the door slowly creaked open, gesturing me to immediately make a run for it. And so I did, without question. I didn’t know if Raven intentionally did that, but I still left that damned place anyway. I wasn’t taking any chances anymore.

Placing both my hands behind my head, I exhaled heavily. I had to inform Vinny about what happened. Either way, I knew for a fact that this would be the end of us, unfortunately. I had some decent amount of funds available in my bank account to pay for several nights at a hotel until I was able to get this situated.

As the sun started to rise, I went to Vinny’s. I braced myself for the outcome of our future together. He seemed to be in a relaxed mood as he was frying some Italian sausages. I gathered the courage in informing him of last night’s occurrence with Raven.

His reaction was just as expected–disappointed.

“Vinny, I’m really sorry. I don’t know what else to say. I know, I fucked up-“

“You fucked up, yeah, you fucked up,” he said disappointedly, as the small thick sausages were sizzling collectively, ultimately replacing the awkward silence that ensued.

“But I’m alright now. I can be trusted now, Vinny. On my life, on my mother, on everything, I’m OK. I’m clean.”

Vinny let out a disgruntled sigh. He looked at me, piercing my very eyes with those unforgettable looks of absolute grimace.

“You looked into my eyes and you lied to me,” Vinny said, continuously stirring the pan of sausages. “You treated me like a fuckin’ jerk, like I was never nothin’ to you.”

“Vinny, after what you said, I couldn’t come to you. I-I-I was ashamed. I-I’m ashamed now. But you’re all I’ve got, Vinny..and I really, really need your help. I really do.” I said, lowering my head in complete shame.

He gazed back at me with slight pity. He pulled out a seemingly thin roll of cash, holding it out for me to take.

“Here. Take this,” he said.

I took the money. I counted each bill as my eyes began to tear up knowing that I’d lose Vinny for good. I was dead to him. Whatever would happen to me, he’d just turn a blind eye on me thereon.

“Now I gotta turn my back on you.” he said with utter conviction.

Five-hundred bucks. That’s what he gave me. Five-hundred bucks for a lifetime. It was nowhere near enough to cover the cost of my future coffin.

I shamefully wiped away my tears, feeling utterly hopeless in the shitshow I’ve fully immersed myself in. I didn’t say anything in return. I swiftly exhaled and parted my ways with Vinny thereon. I was scared, defeated. Aside from Franky maybe, I couldn’t go to Tommy and them because their trips were, for some reason, “extended”. Whatever the fuck that meant, it didn’t matter. I had only myself to figure this shit out before it got any worse.

I went back home. The TV’s malfunctions from Raven’s sudden disappearance caused it to be irreparably destroyed. It was utterly useless. I didn’t bother to dispose of the damned thing; it was the least of my concerns, knowing that some demon had it in for me. Raven was determined to make my life bitter hell.
I laid down and went to sleep soundly. I woke up at the exact same time as I did that one night. Lo and behold, it was Raven. She somehow made a sudden appearance on my irreparably destroyed TV.

“What the fuck do you want from me?” I snapped. Raven didn’t respond. Instead, she winked and started to crawl towards me. I almost coughed out my heart when Raven suddenly crawled out of the TV screen.

As she fully emerged from the screen, I ran to the condemned feline and kicked its head to which the head got stuck upwards as if she were staring at the ceiling. I went in for another kick, decapitating her. I was slightly surprised when no blood appeared to have oozed from the severance point. She must’ve been that dead.

Initially, I thought I “defeated” her as I stared at the seemingly lifeless head as it slowly closed its eyes.

When I thought it was all over, I was dead wrong. The head then suddenly grinned sinisterly, and its eyes opened. I stared in absolute distraught and horror as the head quickly made its way back to the feline’s neck, ultimately reattaching itself. I gagged, as that was one of the most disgusting moments in my life. It felt totally unnatural and disturbing witnessing that vile moment.

Once Raven fully came to life, she then lunged at me with full force, landing her jaws onto one of my balled fists. Fearing for my life, I grossly tried to shake her as she bit off not one, but two of my fingers. Gushes of blood immediately oozed from my missing fingers, and I made a dash out of my room AND my house just in time before Raven locked me in again. I immediately dialed for medical services to which they promptly arrived just in time before my hand had to be amputated.

At the nearest hospital, one of the doctors questioned the cause of the loss of my fingers. Knowing fully well that such professionals wouldn’t give my story any semblance of credibility, I just told them that I had an “accident” with some machinery. Thankfully, the questioning ceased and I made a decent recovery therein.

Despite Vinny wanting absolutely nothing to do with me, I still wanted to check up on him to see how he was doing, so I prompted to use the hospital’s telephone booth and made the call there. As the ringing finally ended, I was greeted by none other than Raven herself.

“He’s gone, and there’s nothing you can do about it, Tony.” She spoke.

“What d’you mean? What d’you mean?” I asked, perplexed.

“He’s gone. And soon, you’ll join me.”

The call then terminated itself. With this unbearable anguish tormenting my mind, I started to slam the phone on the booth several times. “Fuck.”

I continued slamming the phone until some personnel came by to check on me. “I knew it. I can’t fuckin’ believe it. I can’t fuckin’-” I said, right before sobbing silently. It wasn’t the hospital personnel, it was Franky, my old friend, checking on me.

“What happened? What happened?” he asked, concerned.

I wiped away my ears and sniffed. “She killed him. She fuckin’ killed him.”

“Aw, fuck.” Franky responded, placing his hand behind his head. Although it was pretty obvious that he wasn’t emotionally distraught as I was, he knew who I was referring to. He even patted my shoulder and offered me a place to stay. I politely declined. Too much was going on in my mind and I had to take care of this before Raven would kill anyone else.

Hours later, I went to my local church. Much to my dismay, none of the priests there were experienced with handling exorcisms or any kind of demonic cleansing, except one: Father Fred Cicero.

The only problem was, Fred was a disaster; this guy could fuck up a cup of coffee. Fred isn’t highly viewed because according to the church, he was getting his second DUI. Choiceless, I sought to get his help.

I knew Raven was still after me. I knew she’s capable of manifesting herself via digital and electronic devices, so I informed Fred all this. With him bearing that, I strongly warned him not to use any electrical devices in my presence, too.

“Yeah, yeah, yeah,” he said, assuring me of his understanding of Raven’s powers and the emphasis of my warning.

“We have to delay the blessing. I can’t do this without my lucky hat, y’know?” Fred said.

In absolute disbelief, I snapped back at him. “Forget your fuckin’ hat. What, are you fuckin’ kiddin’ me?”

Wouldn’t you believe it, he possibly risked another DUI on his way home, just so he could retrieve his “lucky hat”. I swear on my life, when he came back, he wore a New York Yankees hat. Dismissing that, we proceeded with the blessing, or whatever you want to call it.

Now for the best part, and I couldn’t believe this shit: so what does Fred do during the blessing? After everything I told him, after all his “yeah, yeah, yeah” bullshit, he proceeded to use HIS phone during the blessing.

Now, if Raven was watching me, which she most likely was, she’d know everything. She’d know that I was trying to get rid of her, all thanks to him. Suddenly, the expected happened: A deafening tone unleashed itself from Fred’s phone, marking Raven’s presence.

She teleported between Fred and me. She was there.

“Look at this! Thank you so much, Mr. Cicero, how considerate of you,” Raven said, mocking Fred. She took a closer step towards him, and Fred stared at her in awe.

“Tha-that’s my mother’s phone!” Fred said, attempting to save himself but utterly failing. What a fuckin’ balloon head.

Suddenly realizing his colossal fuckup, Fred collapsed unexpectedly. Poor Fred, he got so scared, he had a heart attack and dropped dead right in front of us.
I deeply stared at Raven as she did me. I grabbed the nearest bottle of holy water and poured some of it on her. At first, I thought it wasn’t going to work, but much to my surprise, the feline let out a sickening shriek from its sheer pain as the blessed liquid was taking effect. I then dumped the entirety of the bottle’s contents and Raven writhed painfully as I covered my ears.

She then vanished in mid air. I immediately crafted another makeshift ouija board and asked if anybody was with me in the church. As I got the “yes”, I said goodbye and vowed to never, ever, act on my urges to seek the mysterious. I didn’t even care about the popularity that the video generated, I just wanted this whole mess to be over.

And…it was.

Weeks later, I was at Fred’s grave. “In loving memory of Fred Cicero, 2010”, the tombstone read. I was finally feeling much better and felt more comfortable sleeping in the vast darkness in my room. I checked my phone for any notifications, and I got this:

RAVEN: Once I capture you, you will be forever mine. I will torture every ounce of innocence you have in your sniveling human body. I can guarantee that.
I shook my head and slightly chuckled. I somehow knew Raven was trying to lure me to get her back to my realm, so I blocked her.

I then put on a pair of shades and lit a cigarette. I’m probably more fucked than I have ever been by the time I’m in the afterlife, but I really didn’t care. Soon, I’d go back home and take it easy. I’d enjoy my life before succumbing to my inevitable fate. Whatever. If you’re desperately trying to get rid of any demon, holy water works wonders.

Trust me.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "9fd6f1e8-ccf1-4a78-b3f1-c70bdbd2733e",
                    ThreadTypeId = 1,
                    Title = "No Face, No Case",
                    Content = @"
My name is detective Derek Wallace, of the Cook County Sheriff’s Department, in Cook County, Illinois, and for the last two and a half months I have been assigned to one of the most brutal, and confounding cases that I have seen in my time as an investigator. Admittedly, I haven’t been a detective quite as long as some of my colleagues, but I highly doubt anything that comes along the rest of my career will ever top this.

It all started on a relatively normal Thursday evening. I was sat in my office getting ready to go home, when there came a knock at my door. I got up, and went to open it, only to be greeted by Captain Woods, an older, balding man, who had a glum look on his face. I inquired as to what was wrong. He didn’t answer, he just handed me a case file, and walked away without saying a word. Strange, I get that Captain Woods wasn’t a big talker, but that was unusual, even for him. I wondered what could have possibly been in that file that had him so dour and flustered. I sat back down at my desk, lamenting that whatever may be in this folder was likely going to keep at the office even longer, and would just delay me further from going home, and seeing my beautiful wife, Mallory. When I opened the file, I was greeted with some of the most vile, and obscene pictures and information that I had ever come across, in my 15 years on the force.

First, I read the initial police report, describing a crime scene, in which the body of an older male, labeled as a “John Doe,” was found in an alley between two apartment complexes, with noted facial trauma. Nothing too unusual so far. I mean I’ve dealt with plenty of murder cases, and seen some pretty grisly stuff, but this went well beyond anything I had ever experienced. Paperclipped to the back of the folder were several photographs, exhibiting some of the most unsettling brutality I could have imagined. The body in question was laid on its side, fully dressed, but the man had no face. Now, when I say he had no face, I don’t mean that his face was so badly beaten that he was unrecognizable. He literally had no face. As in, it had been removed. Not with any degree of surgical precision, or anything. It appeared to have been completely sawn off; bisected from ear-to-ear. So, there I was, sat at my desk on a Thursday evening, gazing into the inside of a man’s skull, I had to fight back the urge to vomit, with all my might. No wonder it was a John Doe case, I thought. It’s kind of hard to identify a person when they don’t have a face.

I got very little sleep that night. It was extremely hard to fall asleep, knowing that someone who could do that to another human being was out there, roaming free.
Over the course of the next six weeks several cases came across my desk that followed the same pattern. First, was a woman who appeared to be in her 20’s, found beside a dumpster, outside of Evanston, face seemingly axed clean off, just like the first case. She too, was left without a discernible identity.

Next, came a very confounding case. This time, an elderly woman was found dead in her bed, in a nursing home, with her face cut completely off. When I went to the nursing home to investigate, I asked if the woman had had any visitors that day, but the nurse at the front desk told me that she hadn’t, or that at least no one had signed in to visit her. One positive thing that came from it however, was an identification. The victim was 89-year-old Mildred Harmon, but without any positive IDs on the previous two victims, it was impossible to establish if there was any link between the victims.

The fourth victim was a homeless man, whose body was found in an abandoned warehouse, in Chicago. While I haven’t dealt with too many cases involving homeless people, I will say, they are generally hard to identify under ideal circumstances, what with them generally having very few close social ties, and family, and all. However, this likely rules out the victims being related. They just seem too random and spread out.

The final two victims were a couple, found in a car, still strapped into the front seats, in the parking lot of a strip mall, in Schaumburg. We ran the license plate on the car, and it came back registered to a man named Eugene Lewis. Could that have been the man in the car? Hard to say, without a face. However, when we looked into Mr. Lewis, we didn’t find a record of him being married, so, as to the identity of the woman he was with, we were at a loss. With one ID, and a good solid lead, we looked into any possible relation between Mildred Harmon and Eugene Lewis, but there was none. They were just two random victims.

It was after this discovery that we went to the newspaper and made our findings public. At first it seemed better to keep this quiet, but now it seemed to be getting serious. We warned the citizens of the greater Chicago area to be on the lookout for any suspicious activity relating to these attacks. Without knowing exactly what the murder weapon was, we advised people to remain vigilant for anybody possessing, or wielding any type of sharp object akin to an axe, or a sawblade of any sort. We concluded the press release with a plea for citizens to call our office with any tips or information they had that may lead to an arrest in the case.

It was a little over a month before anything came up again. In that time period, no other bodies were recovered with missing faces. Figured our press release may have scared off whoever was doing this. Unfortunately, just as before, I was preparing to go home for the evening, when my phone rang. On the other end was a woman who sounded a mixture of concerned, and uncertain. She told me that her son and one of his friends were out riding their bikes around, admittedly trespassing on some land they figured they probably shouldn’t be on. I honestly couldn’t care less about some minor trespassing by some boys, when we had a possible serial killer on the loose. She then went on to say that her son and his friend stumbled upon a house in a clearing, just outside of Ford Heights, that they claim had a fence that was decorated in some all-too-lifelike human faces. Now, I have been called to investigate some very realistic, and intricate Halloween decorations before, only to walk away having had a good chuckle with the owner of the property, and usually my compliments on their work, and craftsmanship, and it was that time of year after all, but given recent events, I decided to head down that way, and see what there was to see. I asked the woman exactly where the house was. She said that her son told her that it was just beyond a clearing behind a neighborhood on the east side of the town. So, I packed up my stuff, got in my car, and headed out. It took about an hour and twenty minutes to find the place.

What I saw that night was by far and away the most horrendously morbid thing I have ever seen in my time as a detective. I approached the front gate only to see real human faces adorning the spikes of the gate, like some kind of sick ornamentation. The faces were in various states of wilt and decay, but they were mostly very well preserved, almost frighteningly so, but there they were. I was given a photo of Mildred Harmon by her family, and had one of Eugene Hill after running his plate, and there they both were. I recognized them. There was Mildred’s gentle face, upon this person’s fence, and just a few spikes over, was Eugene. Next to him was the face of a woman. I could guess it was the woman that was found in the car next to him, but no telling, really.

Next, I went up to the house, armed with my gun, expecting the absolute worst. I knocked on the door, receiving no answer. So, left with very few options, I kicked the door in and entered, and announced my presence, only to be met with complete silence. It was dark, and extremely dingy inside, with an overwhelmingly acrid odor. It looked as though no one had lived there in decades. I felt around the wall looking for a switch. When I found one, I flicked it on to reveal something of nightmarish proportions. There were faces littered everywhere. I was only aware of six cases. Who were all these people? There were faces on the banister, on the sofa, in the sink, everywhere. This time I couldn’t hold it in, I went outside and threw up on the front porch.

I collected myself, went back in, and searched the whole house from top to bottom. The place was in utter disarray. One notable find, was that I found a bunch of discarded Sawzall blades. So, that just about lowers it down as to what the murder weapon was; no Sawzall, though. Something of note about the house was that it showed signs of the occupant, or should I say former occupant, having left in a hurry. There was a piece of toast still in the toaster, and a bathtub full of now very cold, and slimy water. The icing on the cake, however, was that on the kitchen counter was a newspaper dated September 2nd, 2021, the day our press release was published, alerting the public to this person’s crimes. There my name and photo was, prominently displayed on a newspaper in this psycho’s kitchen. Whatever, I thought. This person was long gone.

I scoured every inch of that house, and still couldn’t find even the slightest hint as to who the person was, that was doing this. It was frustrating, and honestly quite perplexing. Usually suspects leave some sort of evidence behind, but this person just seemed to be immaculately elusive. No fingerprints left behind, nor any hair fibers that could be easily distinguished from those of their nameless victims, and who knows how many victims this person had in total, anyway. Feeling defeated, I exited the house, hoping never to have to return.

As I left, I called my findings in to Captain Woods. Then, I got in my car, and drove home. I pulled in the driveway around 11:30 that night. I went inside, and poured myself a stiff bourbon and ginger ale, figured I’d earned it after the day’s events. I eventually went upstairs, did my nightly routine, and crawled into bed next to my wife. Mallory must have had an exhausting day, because she was totally conked out, I’m talking no movement at all. Oh well, I figure I’ll tell her all about it tomorrow morning.

Sleep was hard to attain again that night, but right before I drifted off to sleep, I had one torturously haunting thought. Despite all the faceless corpses that were found, and the faces that were found to match them, we still had no case.

Also, what’s that whirring sound?
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e98d96ea-7c4a-4228-8373-f9f6bb3a8880",
                    ThreadTypeId = 1,
                    Title = "Digger of the Dead",
                    Content = @"
Many people ask me why I would ever choose the field of grave digging as my chosen career. Why choose such a depressing and creepy job when I could be almost anything else? Well to be honest I never understood the aversion to it. Death is something we all face at some point or another. It is the one certainty in life, so why do we fear it so much? To me death is a beautiful transition of existence from one plane to another. I am deeply honored to help provide peace to the deceased and their loved ones. Well others may look at a graveyard and get chills down their spine, I see it as a place of peace and tranquility. That was before tonight, before I knew the truth beneath the surface.

I work for the Hemlock Hill cemetery, one of the oldest and largest graveyards in North America. There are over 100,000 graves, some of which date back to the late 1700’s. There is real history here both above and below the grounds. However it seems that even the mighty have limits as the cemetery is running out of room with only a small section near the back left to bury loved ones. I was assigned to dig the grave for one of our last residents. The man was a well liked and respected member of the community. He was a giant man at over 6 foot 5 and well over 300 pounds. The casket was bigger then some automobiles so a simple 6 foot deep grave wasn’t going to cut it. If I was going to bury this man it would be with kindness and dignity, in a grave worthy of his greatness.

We had been experiencing a record heat wave this August so digging during the day would have been a virtual death sentence. I choose to bury him in the relative cool of the night. Relative was the right word for I was still sweating profusely even just operating the tractor to dig the grave. When I had gotten a satisfactory hole dug in the ground I choose to take a break before doing the shovel detail. I pulled out my cigarettes and had just lit one up when I could swear that I felt eyes on me. To satisfy my childlike curiosity I looked around only to confirm that I was indeed alone. There was no living person for miles, just my newly deceased friend. I looked to the oversized casket and felt as if the eyes on me were coming from inside.

I glanced down at my cigarette and chuckled. “Hey don’t you judge me, I’m not the one in the box. We all end up in a hole someday so what does it matter if I get there a little faster?” I chucked the remainder of my smoke and took a swig of water. I let out a sigh and looked to the casket again. “I see you are in a hurry to get in your new home my friend. Let’s me finish it up for you real quick.” I threw my jacket to the ground near the tractor because even with the sun down it was still way too hot to wear it. I placed my ladder at the edge of the hole and set up a flood light to illuminate up the dig site. I grabbed my shovel and began the slow decent into the grave.

Slowly but surely I began carving out a decent sized square hole. It was the deepest grave I had ever dug and to be honest I felt a hint of pride. However when I stuck my shovel in the middle one of my worst fears came to life. I hit a pocket of loose soil and faster then a landslide the dirt beneath me began to cave in. I had heard of these events occurring during archaeological digs and it often ended in tragedy for the digger. The dirt around my feet began break at a rapid rate and I was sent into a panic. I jammed my shovel into the ground but it took no time at all before that ground broke too and I was in a full blown cave in. I screamed for my life and began clawing at the soil engulfing me. My efforts were in vain for the more I struggled the looser the dirt became. With one final groan the earth swallowed me whole. The last thing I saw before landing was my flood light falling from it’s perch and breaking, casting me into complete darkness.

I hit the ground with a mighty thud and likely would have broken my back if not for the soft dirt pile underneath. Streams of dirt fell over top of me and my breath came out in ragged bursts. One sort of irony in my work is that I feared being buried alive above all else. Now here on this moonless night I was suffering my worst fate. However by miracle and grace of god, the cave in settled itself stopping the flow of dirt. The good news was I wouldn’t die in a cave in but the bad news was I was now stuck several feet beneath the surface. I was at the deepest section of the cemetery with no one around for miles. I was completely and utterly stranded.

“No, no I will be fine. I’ll just call for help.” I patted my chest and was horrified to find no comforting bump of my cellphone. The phone was in the inside pocket of my jacket which now rested several feet above me and well out of my reach. Despite the heat I felt a shiver run down my spine. “I’ll just have to climb out and be very careful.” I slowly reached out and patted the dirt. It was soft but I had to try, my life depended on it. Trying to get solid grip was like trying to put a body of water in a headlock. Every grasp of my fingers was met with dirt slipping through my fingers like sand in a….in a. “In an hour glass.” a cruel and macabre voice in my mind responded. “How many falling sands do you think you have left?”

I shook my head vigorously. “No I am getting out of here!” I grabbed the dirt wall as hard as I could and frantically tried to climb. My efforts were rewarded with a fresh shower of dirt from the surface caving in on me. I panicked but thankfully the shower was short. I had to suppress a whimper because now I knew that climbing out would only quicken my demise. “Trapped in one of your finest graves, how is that for irony? Did you dig this one for yourself?” The horrible voice taunted. I placed my hands over my ears and shook my head. “Shut up I am going to be perfectly fine. It should only be a few hours until morning and then someone will find me and send for rescue.” That cruel inner voice actually had the audacity to laugh at this. “It’s the weekend, no one is coming to rescue you for days. You are alone and you will die alone in here.”

My heart began pounded in my chest like a jackhammer. The stupid voice was right, no one was due to come look for me for a while. I had no family that would notice my absence. I truly was completely alone in this cold hard earth. I smacked myself trying to clear the dark thoughts. “Calm down you can do this. The human body can go days without food or water. I just need to wait it out that’s all.” I nodded my assurance but my inner demon was not convinced. “Sure under normal conditions you can go days without water. However in an august heatwave not to mention underground, do you really think you will last long? That blazing sun will be up before you know it ready to fry you like a can of beans. It’s so sad to think that there is a canister of water pretty much right above your head but you will never reach it.”

I wanted to cry but that would only serve to dehydrate me faster. I wanted to punch the walls and scream into the night but that too may just kill me faster. “Since when have you cared how fast or slow you die? We all end up in a hole, what does it matter if I get there a little faster.” This time I did cry, I sobbed into the swarming darkness. I was in a literal pit of despair from which there was no escape. “You are all alone in the dark. Soon to be a snack for the worms to feast on. Do you feel them in the darkness swarming all over you, worming their way into your soul?” The sad thing was I did feel them. Within seconds thousands upon millions of worms were crawling out of the soil and were crawling all over me. I could literally feel their smiley bodies wiggle against my skin. They were not alone either, oh no. Out of the cracks of hell all manner of crawlers emerged to devour me. Spiders, centipedes and every other dark creature imaginable were rushing over me. I couldn’t control myself and let out a scream.

I began smacking myself all over my body and that was when I felt it. I smacked my pocket and felt perhaps the one shred of hope in this hellhole. My trusty zippo lighter pressed against my thigh just waiting to be ignited. I reached in and lit it up like a Christmas tree. In seconds the light of the golden flame lit up the enclosed space, banishing all the imagined crawlers from my sight. I felt hope light in me anew. I believe in that moment I understood how man must have felt gazing upon fire for the first time. It was such a simple thing to be able to see and yet it did wonders for my soul. “What say you now demon voice? Going to ruin this feeling too?” The voice was for the first time silenced.

I leaned back and just stared at the flame for a moment. I used it to light my surroundings and was surprised to find that it’s light seemed to expand beyond my initial expectations. Pushing the flame further showed me an arc in the soil leading to another chamber. I crawled through the narrow arch to discover more and more arches everywhere. This wasn’t just a hole, it was a series of catacombs. The tunnels were far too large for an average animal to create so were they natural? I knew I had read about antechambers existing underneath volcanoes but underneath a cemetery?

I pushed my light farther and saw something gleaming in the darkness. At first I feared that perhaps it was a set of eyes or the scales of some serpent. I felt that childlike fear stretch over me once more and every part of me wanted to run. Instead I inched forward to discover that the gleam was actually wood, very familiar wood. It was a Cherry wood casket I had buried in the ground not a few weeks ago. It belonged to Mr. Tilman of the nearby Bakersfield. It looked like the casket had fallen straight through the ground. I felt a strong sense of sadness fall over me. I put this person to rest and the thought of it being disturbed horrified me. “I’m so sorry Mr. Tilman we are going to make this right for you.” I placed my hand on the lid and gently opened it wishing to pay respects to him directly but what I saw chilled my blood instead.

Perhaps it was closer to say what I didn’t see, for the body was gone. I panicked and searched around the area dreading to find his body simply laying in the dirt. However the dirt gave no token of his appearance. “He couldn’t have decomposed that quickly, it’s impossible.” I checked back in the casket hoping that my fear riddled mind imagined the absence but no all that remained were some strips of his burial suit. Fear and desperation rushed through me again but also inspiration. “I’m so sorry for this.” I closed my eyes to my unholy deed as I ripped off a damaged piece of the casket and took out the strips of cloth. I wrapped the scraps around the piece of wood and lit it on fire making a make shift torch. I pushed the torch to the middle of the arches only to illuminate my worst fear. All around the area were the other caskets I had buried in recent months. All of them were forced open, some with doors hanging off their hinges. In all the ones I could see into the bodies were completely absent.

“What could have done this?” I whispered into the dark. “Ber…bal…ang” the horrible inner voice whispered back slowly. It was almost as if it feared the words he spoke. They were words I didn’t understand however they felt familiar, eerily familiar. I inched my way forward and examined the caskets. Upon moving one of the broken lids I saw what looked like long and deep claw marks embedded in the wood. I recoiled back and panted deeply. “Berbal…ang” the voice repeated. I had no idea what made those marks and a big part of me didn’t want to find out. However if there were tunnels perhaps one led out. It was my only shred of hope at this point so I had to go on.

I crawled for what felt like miles and the further I went the more questions arose. What made these tunnels? Do they go beneath the whole cemetery? The whole county? The whole country? However there were two questions that plagued my mind more then any. What took those bodies and more importantly why? These tunnels were way too complex and unorthodox for organ harvesters and especially for necrophilies. Who would go to this much effort instead of just going through the top? “The question you should be asking is what left those claw marks.” I chose to ignore the voice this time for there were questions I certainly did not want answers to, not now, not ever.

The ground sloped downward which concerned me for I wanted to go up, not further down. I shook my head because I had come too far and had to keep moving. As the ground lowered the temperature rose. As well there was the beginnings of an odor in the air. The scent was unpleasant but not unfamiliar. I closed my eyes and sent up a brief prayer that the smell wasn’t what I thought it was. For every gravedigger, every mortician knew that smell well. It was the smell of decay, the smell of death. The further I went the stronger the smell became until it was almost unbearable.

In addition to the heat and stench a new problem arose. From straight ahead I began to hear a series of sounds. I tried my best to make sense of what I was hearing. It sounded like the sound of cardboard being slowly ripped by hand. There was also an undertone of water or simple dampness as if the cardboard was submerged in water before ripping. I didn’t want to know what the sound was, I just wanted out. “Please go back to the grave now before it’s too late.” The inner voice pleaded. This time it sounded legitimately afraid and caring. I had never wanted to agree with that voice more in my life however I doubted I could find my way back if I tried. I had to go forward no matter the chill running though my bones. I inched my way forward and nearly froze when I came across movement straight ahead. I slowly lifted my torch on shaking hands. The flame illuminated the source of the movement and I had to jam my hand against my mouth to stifle a scream.

Bathed in the light of the torch was a creature of untold nightmares. It’s appearance gave off only the slightest impressions of humanity. It’s skin was pure white as if this creature had never seen the sunlight. It’s head was framed by a set of bat like ears. The face was all hard points giving the impression of spikes about to burst free. It’s eyes were a complete milky white and I knew in an instant that it was blind. It was not naked but wore black tattered cloth with small hints of shredded white. It was a funeral suit I realized in an instant and the need to scream rushed over me again. It’s hands bore long claws that were bound close together and looked very strong. “They are like a badger’s claws. That is how it made these tunnels.” I thought to myself in disbelief.

On it’s arms it bore something that I initially thought was more shredded cloth however closer inspection revealed something far more horrifying. Wings, this creature had wings attached to it’s forearms like a bat. However despite it’s horrifying appearance it was it’s actions that disturbed me more. The beast was holding the remains of what I could only assume was the late Mr. Tilman. The creature raised it’s joint set of claws and made a quick swiping motion cutting off Tilman’s leg. That horrible ripping sound assaulted my senses as flesh was ripped from bones. What it did next would haunt me for what remains of my life. It’s jaw unhinged revealing multiple sets of razor sharp teeth. It took hold of Tilman’s leg and began shoving it down it’s throat with extreme ease. What happened to his leg and where did it go were things I couldn’t even bear to think about. The creature made chewing motions and rehinged his jaw before going onto the next limb.

“Berbalang.” That horrible inner voice said once again and at once I was filled with absolute clarity. I knew what this horrible monstrosity was. My grandmother was raised in the Philippines and she would tell me stories and folklore of her home land. The story that always scared me the most was that of the Berbalang. It was a horrible ghoul that preyed not upon the living but the dead. An immortal being that fed upon corpses. Though it preferred dead bodies it would feed upon the living in dire situations. How long had this creature been living here underneath our very feet? How many tributes have I placed in the ground for it’s waiting jaws? How many centuries had we been giving it a never ending buffet of dead bodies?

But that buffet was ending wasn’t it? The cemetery had reached it’s limit and soon the food supply would run out. Would it venture forth then for fresher meat? And did I not just give it a perfect tunnel to fly out into the night with? At the horrifying realization I let out a loud gasp and the creature responded. It dropped it’s meal and began scenting the air. I shook my head back and forth. “No it can’t smell me, not with the smell of death dominating the area.” I told myself just begging for it to be true. The fiend continued to sniff the air getting closer and closer. It reached out with clawed feet, feeling out the area around it. The creature stopped searching mere inches from me and I held in my sigh of relief.

The creature turned it’s head to the side in a sickly human way. It looked like a curious child. Without warning his head came forward and he let out an ear splitting screech. With all those bat like features how did I not even consider echo location as a method for it to see? The creature knew where I was and lunged at me. Without thought I swung my homemade torch right into the creatures face. The beast recoiled and started to bat away the flames. I wasn’t going to stick around to see if it succeeded and ran for it. With the fading light of the flames behind me I could only just barely make out the arches of the tunnel. Relying on instinct to guide me I made my way back to the hole with as much speed as I dared.

From behind me I heard another of those horrible screeches. The sound of drumming footsteps followed suit and I knew it was giving chase. This thing had the advantage. This was his domain, his lair, I only prayed I had enough of a head start. I ducked though arches, never sticking to a straight line. Without warning a loud screech emanated from the darkness and the back of my calf erupted in sharp pain. The nightmare had caught up to me and sliced my leg, but only a glancing blow. I thrust my leg back and could feel my foot collide with the creatures bony face. I hobbled as fast as I could as the creature fought to regain itself. I had one chance to end this and one chance only.

By chance or perhaps fate I could make out the chamber I entered by the dull light of summer predawn. I ran with all my might and stood below the open grave. From the depths of darkness I could hear the ghoul gaining ground. “Good let him come. I won’t let him hurt anyone ever again.” I shouted to the ghosts of the dead in the cavern. I looked up at the navy blue sky one last time. What I wouldn’t give to feel an evening breeze once more before the end. It didn’t matter for this was my end and I was going to make it count. I found myself dwelling on the one bit of wisdom I had collected over my years of digging graves. We all end up in a hole someday. “So what does it matter if I get there a little faster.”

The creature leapt out of the darkness at me. It’s claws gleamed in the dawning light, preparing to rend me into a thousand pieces. This was the moment I waited for. I pulled both my fists back as hard as I could slamming into the fragile walls of the cavern. I could feel the earth shake around me and knew it was now or never. I thrust my arms forward and wrapped them around the creature. Using the strength given to me from digging a hundred graves I locked the demon to my body and forced us both to the ground. Above us dirt and rock cascaded down upon us, burying all that we were under it’s unyielding pressure.

I can feel the pressure crushing my organs as my lungs will with dirt. My inevitability has finally come and I feel myself slipping into the next world. However as my heart beat grows weaker, the Berbalang’s heart stays steady. It has survived my trap and lies in wait. I only hope my body can work as a prison long enough for it to starve…..I…..can…..only….hope.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "f1767a58-5c40-4a1d-b256-bb96fa60d9ea",
                    ThreadTypeId = 1,
                    Title = "The Light Hungers",
                    Content = @"
t has been the way of things since the dawn of time. Evil has always been birthed by darkness. The strong feed on the weak and the lesser. Not always for survival, but sometimes, just to revel in its cruel power. To inflict suffering for the sheer pleasure of it. Though the methods and locations may change over the long eons, the story is always the same.

The creature crept from the shadows towards the little girl. For the last few hours, it had toyed with her, meticulously building her fear to deliciously intoxicating levels. The darkness was strength, power, and the creature’s to command. Patiently it watched her the last two nights, learning her hopes and fears. Oh, how it loved to let its victims desperately grasp at hope after playing on their fears for a while. Then letting them feel a hint of relief before snatching it away. Crushing them under the weight of their terror when it moved in for the kill. As the creature got closer, it could see her trembling under the blanket.

“Always the same,” smiled the creature, “hiding under their blanket as if it could keep out the darkness or shield them from my claws.”

Slowly the creature cut the blanket with its black talons, careful to only graze her soft vulnerable flesh. Its mouth watered as it waited for the inevitable scream to burst forth from its sobbing victim. The creature always delighted in when its victims finally realized at the end, that prey has no hope.  The creature readied its claws for the strike, pausing only momentary, to build and savor the dread in its prey before dealing the first one.

“You think you’re so strong,” whispered the tiny voice under the shredded blanket.

Stunned by her words, the creature recoiled and pulled back its claw as if burned by them. Confused by this unexpected behavior, the creature perked up its ears and listened intently. It wasn’t the first time its prey’s fight-or-flight response triggered aggression towards it. There have been times when a mother tried to protect her children or when a man tried to protect his mate. But even on those rare occasions, it was only done out of desperation, and never from such a tiny morsel.

All those false heroes, filled with their fleeting and desperate courage. They were nothing more than a delicacy to the creature, and their despair was seasoning for its meal. They were merely a plaything to disembowel and leave alive long enough to watch it savor the agony of those they failed to protect.

“You think the darkness you wield as a weapon makes you powerful…,” continued the tiny voice.

It was then the creature realized the morsel wasn’t trembling with fear, but giggling!

“…but darkness is weak. It’s nothing more than the absence of light…”

Desperately the creature lashed out at the girl.

“…and held at bay by the smallest and most delicate of flames” said the little girl, halting the creature’s blow with nothing more than a small spark held in the palm of her hand.

Howling in pain and rage the creature clutched its wounded claw to its chest, frantically looking for an opening to end the girl. This was no longer a hunt or a game, but a fight for its survival. “I am not birthed from the darkness; I am darkness incarnate” raged the creature. Shadows from around the room gathered to suffocate the now growing ball of light in the girl’s hand.

“I have hunted and devoured countless lives, feasting on their fear and despair, from time immemorial. Do you think I fear your simple parlor tricks little morsel” sneered the creature. The shadows now blanketed the light, dimming it, giving creature the opening it needed.

The little girl looked up at the creature and smiled, “You still think of this vessel as your prey? I would think something as old as you would be wise enough to know the difference between prey and bait.”

Light poured from the little girl’s body, flooding the room with its brilliance. Searing pain tore into the creature with greater savagery then anything it had ever experienced driving it back under the bed. Huddled in the last remaining shadow under the bed, the creature shook with terror and pain as the light continued to creep towards it to consume its remaining flesh.

In its last moments of existence, the creature sobbed to itself, “light feeds on darkness… light feeds on darkness… light fe…”

Another time, another place. All though the cast may change but the story is still the same.

After carefully cutting the screen out of the window a serial killer quietly slipped inside the sleeping child’s bedroom and silently crept across the room to where the little boy was hiding under the blanket. The killer smiled at the thought of the parent’s finding their precious child butchered by her hand, and savored the pain she knew they would feel. As she readied her ax to paint the room in the blood of her newest masterpiece she heard a tiny voice giggle from under the blanket “You think you’re so strong…”
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "fe8b0fc3-174f-4270-8e77-de2c00e6c47c",
                    ThreadTypeId = 1,
                    Title = "Confessions Of A Fisherman In A Town Called Langurst",
                    Content = @"
Langurst is a small place, quaint and quiet. A seaside village which I’d never seen the outside of. The comforting sound of the salt waves breaking against rock was seldom disturbed by the modern hardships one might find inland, Langurst was a bit behind the times you could say, traditional maybe. Most labor to be found in the village was exactly what would be expected of a place so agricultural and simple, farming, manual labor, lighthouse keeping for a lonely few and fishing most of all, it was what our town was known for after all. It was the kind of coastal refuge where hither-to little ever seemed to happen, so when something finally did it sent shockwaves throughout the old hamlet.

Almost symbolically, a violent wind rose upon the land on the day I got the news. A local boy hadn’t come home the previous evening. His parents had cared for the child in a cottage not two minutes’ walk from my own. Sam his name was, he’d had his sixth birthday within the last month. The story was he had been let out to ride his tricycle along the cobbled streets before dinner. I admit I knew young Sammy very little; I only learnt his name after he disappeared but nevertheless, I found myself among the first to volunteer for the search parties.

Day by day our righteous troop of concerned townsfolk grew until we could sweep the surrounding country in one walk, and each day we would come up empty handed. A week into the search we almost had the whole town on the hunt, spare for an unsavory few. While trudging through the hallowed mud, torch in hand I inquired to my reluctant companion, Randolph, on the noticeably absent Erik Carter, a local eccentric/seadog. “What of him?” replied Randolph. I looked forward into the blackened country before expressing my suspicion. “I just find him strange is all.” I felt Randolph’s gaze turn to me, his brow furrowed in hesitation. “how do you mean, I can’t remember the last time I saw him in town?” He brought up a good point, first one of the day, but Randolph was right to doubt my distrust of the man.

See Erik was a recluse, a rugged old sailor, an intrepid mariner as he was known around Langurst. The man only left his beach hovel to fish out on the infinite blue. He would take his rustic aged boat out to wade in perilous waters which teamed and foamed endlessly beyond our small populace. But, unlike our more precocious fisherman, Erik kept from selling the haul he would bring back and instead stored them for himself in his low wooden shack which rested quietly on the beach. However, it is known that there was a time where he wasn’t such a hermit. In fact our mysterious character was once part of a crew in his younger years.

This was before my time of course, but I had heard the stories. Erik found a love for the ocean on a crew a five while his soul was unscathed, him and four others would brave the thrashing waves and come each day back to the beach with enough fish to feed a small army. An impressive bunch to be sure. But (and this is where the detail is uncertain) one day, as they did as they always had, seeking after shadows of shoal cast by that pallid, peering crescent moon, Erik had an encounter. Or a revelation, or saw a sight which sent him reeling in hysterics, screaming of uncertain, undefined things which no one could decipher. Upon forcing the crew to turn back he exited the grounded boat onto the beach and left, without explanation to his cabin, where he stayed for weeks. Leaving his fellow sailors aghast in confusion, before he finally left and began sailing again with the boat he has kept since, still refusing to talk of what had happened to him to that day.

Erik’s former crew had all but since died, old age had finally sent them back to the weeds, all but one, William Wallows who remains similarly unsocial.

I halted and turned to Randolph. “Maybe it’s wrong of me to accuse a troubled old man but, something about him screams to me that he is not to be trusted, I can’t explain it.” Randolph smirked and grabbed my shoulder. “you’ve been listening to too many stories, that’s your trouble Harry. Come on I’m getting exhausted let’s head back.” Randolph suggested, patting me on the back. I concord with his suggestion before turning and heading back through the farmland. Empty handed once again.

Somber was the following month. As I had done when the mystery was fresh, I spent the weeks of February up in the hills and fields, combing through grass and wood with the rest of the search team. But on March third Sam was presumed dead by the village. A victim of the universe’s cosmic indifference. The search was called off. A ceremony was held in his honor at the cemetery which rested eerily on the towns outskirts as a hideous reminder of Langurst’s often unspoken past.

In the callous silence of that hoary city of sarcophagi, an empty coffin was lowered into the graveyards soaked mud which sat almost mire-like beneath the featureless grey sky. The unnerving noiselessness of the place was only occasionally interrupted by the wet sniffles of the mother, and the father who cradled her in his bemoaned embrace. With a vile squelch the coffin smacked the mud that sat at the bottom of the hole. I hadn’t been to that place in years, but I felt the obligation to go, after all I truly hoped we would find the boy.

To my slight gratitude most of the population of Langurst was in attendance, besides, it was a small town. And once again among the folks who were missing from the event were those too old and frail to walk and, Erik Carter. Even his old pal William Wallows came, peering on from the outskirts of the crowd, reflecting his courteous but faintly aloof nature. I turned to Randolph who stood to my side, hands clasped together by his torso. “Again, look who isn’t here,” I whispered, nudging his elbow. He didn’t reply.
After the sermon had concluded the crowd dispersed, myself first of all. Randolph grabbed my arm as I turned to leave. “What’s the hurry?” he asked intrepidly, slightly concerned. Lying I replied “toilet”, my one-word answer seemed to agitate my anxious friend but nonetheless he left me to my own devices.

I scampered down to the bay, passing the empty houses which stood silent along the way. High up on a rock which overlooked the stretched beach I stood, peering out to the waves. Just hazily, among darkened blue and uneven sea, as if a smudge on a mirror I saw him. On his little boat amidst the howling of distant wind and rocky currents he was. What was it that drew me to him I couldn’t describe for our town wasn’t lacking in eccentrics? But looking out at the faraway speck that was Erik Carter, I couldn’t shake the feeling I had seen him with the boy, whether it was in a dream or just a conjured image of my own mind I still can’t say. I mean, being cooped up in a small fishing village your whole life is sure to leave your mind imaginative. But no matter how it may seem, an unspoken part of me hoped that young Sammy had not just been whisked up by the country or torn asunder by some woodland beast, but instead, wished our intrepid adventure had snatched him up, and that it was my duty to rescue him from the ragged clutches of the scurvy old sea dog.

It was hard to tear my gaze from the timeworn fisherman but once I did, I clambered down from the salty rock and made my way back up the cobbled street. The night came slowly that day. A long sunset crept its way below the waves, sending golden orange rays upon our small town. As the streetlamps became alight and darkness befell the huddled rooftops, I found myself downing a pint in the Singing Siren, Langurst’s very own pub. Its only pub. And before I knew it myself, one pint turned to two then to three and however many after that, I could hardly remember.

I stumbled my way out the old place and looked wistfully down the street which stretched down to the bay. Not quite prepared to go home, I clumsily moped down it. The concept of time was mostly lost to me that night due to my sickly level of intoxication, so it could have been an hour by the time it took me to reach the point where the stone became sand and grit. I meandered on the border of the blackened beach under faint, flickering streetlamps until my vision started to straighten and I began to feel the cool breeze on my skin again. I remember sparing a glance or two to Erik’s distant shack, which emitted a vague orange glow through its stained windows, but no movement could be seen inside. I found myself a nice bench which looked out into sea and sat, ruminating to the sound of soft waves crawling up and down the sand.

Somewhat lost in thought, I almost missed the sound of a quick whistle behind me. I turned to see who made the sound for they were surely trying to grab my attention. What I saw when I did was I sight I did not expect. Emerging from the poorly lit street came the ragged visage of one, William Wallows.

“Mind if I sit with you?” he asked, almost growling in his hardened demeanor. I felt as though saying no would not amount to much. I gestured to the space beside me as he lowered himself onto the bench with a breathy rattle, like a train chugging to a halt. His long brown coat had a sickeningly liquory stench, like huffing pure ethanol. “Nice of you to do what you did for that boy, spending all your time looking for him, very kind you must be.” Despite his undoubtedly intimidating entrance he sounded rather sincere. Though I found it strange of him to say since his contribution was non-existent.

“Well I’m certainly no exception, most the whole town was out to find the boy” I said, still looking onward into the darkness. “yes I suppose they did, good town we have here, even better people, most of em anyway,” William chuckled in response, taking a swig from his flask. “I was terribly sorry to hear about young Sam, I wanted to help I truly did but, something about being out there is just-”. He stumbled on his words for a moment. “it’s being around the dead trunks and stunted trees which rot at the rim, and the vile mud which belches when you stand on it. It’s a place where I do not belong” there was almost a sadness in his tone now. “Is that why you became a fisherman” I asked inquisitively, turning to him. “yes, I suppose that might be it.”

“I think I find a particular kind of comfort in the uncertainty of the waves, in its opaqueness. Most people do not feel this way, I understand that. They fear what they cannot see. They are frightened by what might or might not lurk below. Not me though. I find wonder in the unknown.” He sighed as if he would say more, but he didn’t.

I looked at him, still sat by my side. His eyes seemed hollow, his pupils swayed back and forth absently, as if he was scanning the bay while still not looking at anything at all. If I stayed silent for a few more moments he certainly would’ve passed right out on that bench.

“Was Erik frightened?” I asked. He took a breath before letting out a raspy chuckle. “if only you had known how stupid that question was, you wouldn’t have asked it.” He said.

“how so?” I asked, slightly hesitantly, somehow not the least bit offended by what he said.

“This place is small, have you not seen him out on the waves each day, wading between those waves.” He pointed outwards into the abyss. “I know everyone has heard the story, you too I must assume. I know they say that something in the water spooked him, frightened him into mania. But I was there. Sailed for a long time before then, I’ve seen fear, young sailors falling of boats into the blue, seen their eyes bulge and turn empty like a fish. That wasn’t fear. He screamed certainly but not as any primal instinct or for help. But to tell us something. His message was unclear but what was clear, was the overwhelming madness that was spilling out of his mind. It was like his brain had burst.

I leaned forward as he stopped. “so, what did he see?” I inquired, slightly frightened.

“As I said it was unclear. Adjectives like accused, deep, great and mighty shrieked out of the word soup he had spilled on our boat. We sailed the boat back and dragged him ashore, he screamed all the while. We haven’t spoken since but, he seems to be doing simply fine.” He chuckled.

There was a sullen silence as I stood up, now mostly sober. “where you off to?” he said almost growling without the energy to turn and look at me. “I must be getting home” I sighed, before turning back to face the dimly lit street. Still no closer to finding the boy, but without explanation, on the basis of no evidence or reason whatsoever, even more distrustful of Erik.

The following day came and as it did so did the realization that my infatuation with the lonesome sailor had become unhealthy. I couldn’t rid my mind of him, his story. I had gotten little sleep once I returned from the bay, I could practically feel the bags under my eyes. I left my home in the early hours, just as the sun had come. This exact reason for this I am unsure, I mean, why does anyone go for a walk. It was quick before my walk took me to the bay once again. As I paced on the beach’s edge I scoured the ensuing waters, almost out of instinct. Erik could not be seen. Certainly he was now in his shack which was in my immediate peripheral. It rested not to far up the beach, a lonely structure on the sand. I was ready to pass it when I sensed something, a movement.

The old lantern which hung down inside shook. Then swung. Something inside was shuddering the skeleton of the place but little noise could be heard from where I was standing. Cautiously I moved in closer. Now no closer than five feet from the rear of the shack, something became audible. A muffled cursing could be heard from inside yet, as I peered through the window, hunched in the sand, Erik could not be seen inside. This was my first time hearing his voice. Through the old wood I made out the aggravated groans and expletives of the man among the bangs and rings of metal grinding against rock, which sounded not dissimilar from the jangling of chains. I couldn’t believe what I was hearing, was my half-deluded theory true, was I hearing the proof. With my ear pressed against the rotted planks I could confirm the ruckus came from below.

Was this man who previously had shown little sighs which warranted suspicion really the culprit? Had he been holding the boy in his perverted sub terrain chamber this whole time? Was I really about to rescue a child thought dead? I wondered. But just as I thought I had it all figured out. My victory shattered as my bones rattled.

A sound which my straining ears were not near enough ready for, bellowed from below. A repulsive, gargled, watery belch erupted upwards into the sky. An almost unspeakable blare of grotesque noises which smacked the walls inside, like Neptune’s stomach churning. I recoiled backwards, landing with a thud in the sand. I scrambled in it for a few moments before getting to my feet and shooting off toward the town.

While perhaps having nothing to do with young Sam, it was clear Erik was harboring a black secret. I didn’t stop running until I got back inside my house, but I was too far down the hole to abandon it then. I would wait for the cover of darkness, when Erik was sure to leave his shack to head ocean bound as he always did, to leave me free to uncover whatever clandestine, accursed thing he was holding beneath the sand.

Inevitably darkness came. I had perched myself on a far-off mound of rock where I could get a good view of his front door. The damnably vivid sound of the swaying waves had begun to drive me a little loopy in the head by now, but I wouldn’t let it deter me yet. Down on my stomach I ogled at the repulsive structure, the rotten wood of which could only be made visible by a monotonous golden hue that radiated from the lamp inside. Soon, however, that monotony would be broken by the hunched shadow of our mysterious stranger. He clattered around inside for a few moments before, above the nightly ambience of the beach, a sharp creaking could faintly be heard as Erik opened the door. The lamp which he now held in his hand burst out into the darkness, illuminating the surrounding sand, leaving his oak hovel in darkness.

I pushed myself to my knees as the old man sulked along toward his boat. I admit there was something eerily calming in watching the man live his strange little life. The confidence in which he threw his light before himself aboard was almost impressive. But before I could get too transfixed, he was off on his way, pushing against the tide. I slid down from atop my perch and bounded across the bay toward my target. Leaving as soon as I did gave me a good few hours to search inside I figured, and not caring for subtlety would give me all the time I would need. I felt along the exterior walls of the place before touching glass, I must’ve found the window. With little hesitation I removed my coat and wrapped it around my good hand. With a breath and a gulp, I smashed my enveloped fist through the dirty glass which shattered with a painful screech. I pulled my hand back and came with it did a wretched, eye-watering stench which oozed out the sharp opening I had just created.

I heaved as I breathed in the foul, fishy smell, but again I could not stop. Still wrapped up, I pushed my hand around the rim of the window, removing all the left behind glass before crawling into the place. My feet met the floor with a tender creek, the interior was blackened completely. It was apparent to me then that Erik had taken his only source of light with him. From my hand I unfurled my coat, now torn and shredded beyond repair, and threw it out the opening behind me. I felt around my pockets, finding my lighter in the left one before shining it outwards in my outstretched hand. I had gotten lucky with that one I suppose. The light was dim and small, but it gave me vision at least, well only about two feet of vision, but it was better than stumbling around the nasty place like a blind old hag.

I stepped forward cautiously. The light from my small flame didn’t reach the ground. I stepped again and the wood churned but as it did, my free hand brushed the edge of something, splintered wood. I brought the lighter too it. It was a table for sure, mostly blank bits of paper atop it, but some words could be made out among the tattered pieces. Scrawled in black chalk was a list of some sort, but it was muddled, barley legible, all the words crossed out by a thick obscuring line. I could faintly make out words like shrimp and possum among the crossed ones.

After moving from the table, I stumbled around in near darkness for a few moments before I damn near tripped on some kind of metal contraption in the ground. I composed myself and knelt where I thought it was, shining my lighter towards it. A round and rusted metal hatch it was, this certainly would lead me to the basement, and consequently whatever secret Erik held there. I lifted it with a heave to reveal an untrustworthy looking wooden ladder, the bottom of which my small light did not illuminate.

For the first time that night I hesitated. I had no clue what I even expected to find if not for the boy, which I had ruled out by that point anyway. But stood there atop the ladder I began to feel a cool breeze flow in from the window I had already smashed. ‘It would haunt me forever if I abandoned the journey now’, I thought. I let out a suspire of slight regret before one leg after the other, I descended into the darkness, trying all the while not to let my light go out.

The smell down there was like nothing I had smelt before, it must’ve been where the horrendous stench upstairs originated from. Ten steps down my shoe met the ground with a splash, at the bottom I was ankle deep in water. But it wasn’t a thin liquid like the seas but instead a thick, mire-like substance which seeped through the threads of my shoes, it was tiresome to walk in, hard to bring your feet up from. My arm extended, lighter in hand I felt along the walls with my other one. The walls were wet with some kind of slime which caught on my fingers as I followed it around the room.

I was startled when my hand caught another hatch of some kind. My light towards it I could see that through it was a horribly corroded chain that looked as though (given a good tug) it could break at the hinges. I placed my hand on top and followed that instead. It wasn’t two more steps in the sullied water when before me, within the small orange hue of my lighter, hanging from the roof which I had not the light to see, was a string. A wooden nub on the end of it, like the kind of thing you’d use to turn on the lights in an old bathroom. Perhaps I would find some light after all.

I pocketed my lighter, leaving me now in complete darkness. With one hand still on the chain I felt around in the unlit chamber for the little piece of wood. After inelegantly knocking it aside a few times I finally caught it in my hand. But just as I was about to pull it down, my hairs stood up and my blood ran cold. I felt my skin almost ripple as slowly and deliberately, the chain in my other hand began to shudder. Whatever it was connected to was beginning to wake up. Before fear could get the best of me, I yanked on the light switch and with a pulse and a flicker, a single solitary bulb lit the place entirely. But I wish it didn’t.

In front of me, closer to me than I was to the ladder behind it was. With the place lit it was now known to me that the chain I had been holding was one of many. At least ten of them, all around the room, latched into the flesh of a huge gelatinous creature, a mound of flesh. It was the furthest thing from human I’d ever seen. It had an uncountable number of crying eyes which looked everywhere and nowhere at once, accompanied by flailing, flogging, thrashing mouths that groaned and whimpered in a thousand voices of terror beyond all comprehension. Its infested, blubbery flesh oozed a repugnant blacky bile from every accused chasm which I then realised was what was flooding the chamber. Its many blackened tongues writhed and squirmed and in its many mouths which dribbled the same wretched substance. It was the ultimate abomination. The face of horror itself.

I leapt backwards into the mire with an anguished howl. I darted to my feet as fast as the liquid would allow me and I shot up the ladder as the creature let out a blubbery bellow, which shook the foundations of the place. Once back at the top I threw the latch shut and fell backwards into a shelf leaving me panting on the ground. I almost didn’t notice that the room was now illuminated too. My heart couldn’t sink any lower, but it certainly would have when I heard a faint chuckle to my side. Still gasping for breath, I turned to see the man I had been studying for the past few days, sat at a table, staring me deep in the eyes, chuckling as he did.

“I suppose you’ll be payin for that”, he croaked, still laughing. He pointed to the broken window. “oh I’m just messin, stop being such a little grinch.” He gestured me to sit opposite him. I didn’t move. “you invaded my home young man, the least you could do is sit with me now.” Shaking and barely able to talk, I crawled to my feet before stiffly placing myself in the chair opposite him. “Drink?” he said offering me a beer. I ignored his question and managed too stutter out “w-what was that thing?” “Oh no you first, I seen yer spying on me, asking bout me, what is it boy, what you want with an old lonely sailor like me eh?” his tone was less upbeat now. “I-I thought you took the boy, the missing boy Samuel, I thought you took him.” I responded, still my mind was a mess.

Erik looked down when I mentioned him. “ahhh yes, news doesn’t get to me so fast, but, when I found out I was- , well you know, it was terribly sad what happened, terribly sad. I can think of nothing more distressing as parent then not knowing if your kid is ok or not, nothing.” now he sounded truly sad. “That’s why yer were poking round my house was it?” He asked. I nodded timidly. “Ahh well I can’t fault you there boy, trying to be a hero. But as you might have seen, no boy down there,” he pointed to the hatch.
I looked him in his tired eyes, “what is down there?” I asked again.

“oh dear, you shouldn’t have gotten muddled in all my mess but, I suppose if I don’t tell you now, it’ll drive you mad, like this old boy,” he chuckled patting himself on the belly.

I spoke up, “I’ve heard the stories about you, you know, in town they say you saw something, in the waters, is that what you saw?” I prodded, gesturing to the hatch I had emerged from.

“No son, its vastly more complicated than that. On that fateful day, us five sailors- or was it four, ahh it matters not. On that fateful day us sailors did as we always did, went out into the great blue, trying to do our part for this lovely town, when, when I heard something. The other boys were busy pullin our net up ya see, but as they were pullin, I heard something. A whisper on the wind, carried by the ocean air. I looked outwards into the waves; something was calling to me. ‘Erik’ it called to me ‘Erik’ it called once more ‘look to the depths’.

“look to the depths, w-what does that mean?” I asked, entranced by his tale.

“well, quite literally that my boy. The sirens song called me to the depths, so I did it. Almost hypnotized I peered of the edge of the boat, and, and all worldly sounds faded into air as I saw what lay below. The water wasn’t like it had been, thick and opaque, instead it was clear as crystal. I could see right down to the bottom.”

“And what did you see?” I asked once more.

Erik’s eyes widened almost manically, and a sharp toothy grin crept along his face.

“A mighty city, sunken and ancient but not abandoned. The tops of their great monasteries twinkled like stars as creatures great and small danced in the golden paved streets. The ageless priests chanted their endless song in the grand church which sat at the centre of their mighty kingdom, hidden below the waves. I blacked out after that. when I awoke my crew mates were dragging me along the sand and since that day, I barely spoke to them. Because I had been chosen, not them, I was meant to taste the salt. I needed a part of their world, a fragment. When you get a glimpse of the other side my boy, you can never rid yourself of the great burden that is knowing of it. So, on my lonesome I sailed out there day after day fishing for one. It took years and years of work and carrying the knowledge that this world does not belong to us. You wouldn’t believe what I dreamt of. But eventually I caught one, I bloody caught, one a child of Neptune, and I keep her with me here. To sing to me. And now you share that gift with me.”

I looked to the ground, staring at the lines between the planks, in awe of what was down there. I felt Erik’s gaze on me. “Come on son, we best get you home now,” he said softly. I stood when he did and saying nothing, he led me out the door into the dark, but before he shut the door behind me, I turned to ask him one final question.

“So, what finally did it?”

Erik looked at me puzzled

“So what did it, after all these years. How did you catch it?”

Erik smirked. “well it’s all about the bait my boy, and believe me I tried everything. Tested every kind of meat I could get my hands on, rat, cow, bird, fish, possum, even cat and dog meat. But who would’ve known that the thing that would finally do the trick, was a child’s flesh. A monstrous meal for a monstrous thing, I should’ve realised it sooner.”

He smiled at me softly, almost apologetically but not quite.

“Hey that reminds me, I must be headin back into town soon. She’s due for a good feedin.”
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "b2f87ea3-676e-4e8c-9878-908a24ee4354",
                    ThreadTypeId = 1,
                    Title = "Nell’s House",
                    Content = @"
Everybody’s got a story. I don’t believe in ghosts but if you ever asked me if I’d been somewhere haunted, this is the story I’d tell.

Down the road I lived on as a kid, there was an old dilapidated house. It was small and the yard was always overgrown and bushy. It was surrounded by woods on almost all sides and on the western side there was a pond that you could kind of see from the road.

When we drove to school I would always look out my window for the chairs in the woods near the pond. For as long as I could remember, there had been two chairs in the woods in different areas, but both facing the same way. Every few months they would move a bit, but always in the woods, always facing the same direction. It seemed a little strange to me that they would keep showing up in different places when the house was so obviously abandoned, but I didn’t think about it too much. It was just something fun to keep track of on the way to town.

My parents always told me that this place was Nell’s house. We were not allowed near it because Nell was mean. She was technically my great aunt on my fathers side, but there was some bad blood between her and my grandmother. Mamaw would never talk about it but the story went that Nell had a bad temper and took a disliking to her after she married Charles, Nell’s brother. She even pointed a gun at her and threatened to shoot her if she saw her walking past on the street.

As kids, we used to go right up to the edge of the property and watch, but no one ever went in. There were never any cars out front, no sounds, no signs of life. We would watch for a bit and whisper whatever stories we had about the place which wasn’t much.

The story was that Nell had lived in the house with her mother, father, and two sisters: Mary and Eunice. Mary was straightlaced and quiet, Eunice was simple minded and sweet, and Nell was mean as a snake. She and her mother had actually tried to kill her father by poisoning his coffee for a few months until he caught on. He nearly died and was weak for the rest of his life. At least, that’s what we were told. After his death the house went to his wife, and then to Nell. Judging from the stories I’ve heard about my grandad (he died before I was born) and personal experience with my dad, I wouldn’t be surprised if there had been major physical and mental abuse going on in that house that made Nell the way she was.

Anyways, she later got married and adopted a little girl but surprising no one, Nell had turned out to be a horribly controlling and abusive mother and as soon as she hit 16 the girl left and never set foot in that house again, leaving it and her mother to rot. No idea what happened to the husband but I assume he died.

The daughter later sold the house to my dad after it had been vacant for years. He wanted to keep it in the family since it had been his grandfather’s house. I went with him to clean up the yard and take a look to see if anything inside was worth salvaging. The whole place had a bad feeling to it. The kitchen was the worst. Whenever I was in it I always felt prickly and anxious like when dad was in a bad mood and I could tell I was gonna catch hell. Like there was a storm coming.

The whole house was a total wreck. It had been looted by druggies a few times already due to there being only hook and eye locks on the doors. Old bug infested piles of clothes and furniture, books and cans of unopened food, creepy rotten dolls, you name it- total nightmare shit. There was a very skinny set of stairs at the far side of the house leading to the attic which was more like an open crawl space. It was where transients would camp sometimes in the winter and they would leave mattresses and old blankets from downstairs up there. There were also some old trunks and a tiny rocking horse. I never liked to be up there long. The side furthest from the stairs with the trunks was always dark and smelled horrible.

There were 3 bedrooms in the house (aside from the attic) Eunice’s was filled with toys, dolls and fabric, she liked to sew clothes for children even though she never had any of her own. She was very jolly and her room had a horseshoe hanging above the door- an old superstition to catch luck. She had health issues and died not long after her parents. Nobody knew much about Mary but I think she got married eventually and moved away.

Nell’s room was on the back of the house, basically a kind of screened in porch. The bed that she died in was still there when I went inside. I found out much later (to my surprise, I’d always thought the house was abandoned). That she’d died of cancer when I was nine.

While I was exploring the inside and seeing what loot I could find, dad poked around the barn and talked shop about how he could clean the place up. After looking around the house and finding a few cool knick knacks, I was eager to get out of there so I could clean them up and examine the finds. I went home and didn’t go back by for a few weeks.

My cousins lived closer to the house than we did and a few weeks before we bought the place, they’d gotten two new puppies for their kids to soften the loss of an older pet that had recently died. The puppies were cute, but always running the neighborhood since they refused to keep them penned. One day they went door to door down the neighborhood asking everybody if we’d seen them. Turns out one had been hit in the road, and the other was missing. The kids mourned the dead pup before eventually adopting another dog and nothing was ever found of the other one. Most of us assumed it had either been stolen or ran off, which was common for way out in the country where we lived.

Dad came home one night after clearing the brush around the house and burning it. He looked tired, but not in a bad mood so I asked him how it went over at Nell’s. He and my brother Charlie had decided to tackle draining the old water out of the cistern that day. The cistern was a large well-like thing that was partly under the raised kitchen. They used it for storing up water for the house back in the day. It was as big as a garage, mostly underground, and the half that wasn’t under the house stuck out into the yard but was covered with a wooden lid to keep debris from falling in. It was deep and creepy and I did not go near it because of the stagnant water smell. I joked that if there was a dead body anywhere on the property it would be in the cistern.

When dad and Charlie opened the cistern that day to drain it, they’d been hit by the stench of death. Both avid hunters, they decided to finish the job anyways and fished out all the detritus that was a few feet below the lip of the wall. There were a couple of raccoons, a lot of trash, some clothes and DOGS. A lot of dogs. Including the corpse of the puppy that went missing. They piped out the rest of the debris and water and buried the rest.

I was completely horrified and heartbroken thinking of that puppy swimming until it drowned under the house but the more I thought about it the weirder it was. First off, why were there so many dogs in there? I assume a lot of them were strays trying to get at the water or even the raccoons. If they’d just been thirsty though, the pond was only 30 feet from the house. Maybe they’d jumped in by mistake and couldn’t get back out. It was explainable. But there were other things that weren’t so easy to explain. The walls were slightly domed so if any animal tried to get at the water or fell in, they wouldn’t be able to get out. But the lip of the cistern was well off the ground. Almost 5 feet. The bigger dogs, sure. But there’s no way in hell that little puppy could have jumped in on it’s own. Strangest of all, the cover had been completely on when they went to drain it.

I avoided that place like the plague for a while after that but eventually decided to go back and give it one last pass to see if I could find any more cool junk. This time I walked around the outside towards the woods to avoid seeing the cistern. Remembering my sightseeing days on the way to school, I went into the woods to see if I could find those chairs. I found one after nearly missing it, all covered in leaves and bark. I suddenly got the urge to sit in it and see what was so great about the view that made someone leave it there. I sat down and looked west. It was just trees. Endless trees and quiet. I left the chair in the woods.

I went by a few times after that but it was mostly so dad could brag about all the changes he had made. It still looked the same but the bad feeling was gone.

It still stands to this day. Dad says he’ll fix it up or bulldoze it but he never does. It seems like it’s determined to sit there and crumble to dust. And maybe that’s for the best.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "f3cd7787-ead6-4eae-899c-e5952e4934c1",
                    ThreadTypeId = 1,
                    Title = "Summer Nights",
                    Content = @"
Summer nights in the small college town were something straight out of a Pulp magazine: a risqué combination of nostalgia, science fiction and impossible stories. Every year from May to August, the place was left largely empty due to the legion of students who had skipped town for the break. Vacated dormitories languished across campus and the streets were quiet, even when they shouldn’t be. Time lurched, unbothered. The resulting desolate landscape was beautiful and haunting; the perfect setting for a massacre in a Tarantino film. Only the locals remained for the most part, taking care of the city’s mundane functions and pretending any of it mattered.

I had just finished my Junior year at St. Dominic University and, as usual, going back home abroad was not a feasible option; so I stayed behind working for the school’s Grounds Department digging holes, mowing grass and whacking weeds. The pay was minimum-wage but doing it full time meant I had enough cash to cover rent, buy groceries and even venture the occasional dance with the devil at the local watering hole. Life used to be cheap in small-town America. Every day after sundown, temperatures would drop to double-digits while a cool breeze tempered the hot asphalt. In the absence of a skyline, darkness followed. By the time I’d burned through a joint and drank a six of Modelos sitting on my rooftop, conditions were just perfect to go for a walk. I wandered the streets at night equipped with a 30GB 5th Generation iPod filled with enough angst to get the girl and take revenge on my enemies. Walking carelessly down the middle of the road, submerged under electric guitars high on overdrive pedal effects; every step taken was a reassurance of my dominion over the city in the dark. I was powerful. I was alive. I was wrong.

It was well past the small hours one night when I took a few turns off of 16th St. down rows of ordinary homes, each with their AC unit sticking out the window. At the end of every driveway, decrepit mailboxes withered under a crescent moon. Streetlights were scarce and spirits roamed unopposed. On my earbuds, Matthew Bellamy was just about to hit the high note in “Sing for Absolution” when a pair of creeping lights in front of me caught my attention. It was a vehicle. I moved over to the right and kept walking, expecting it to drive by. Instead, the car slowed down and came to a stop under the streetlight straight ahead. A wisp of smoke tied to a cigarette’s cherry emanated from the open window on the driver’s side.

“Fuck,” I thought. It wasn’t unusual to get harassed by some of the locals on occasion; they were an antiquated, religious bunch and my appearance left much to desire according to their standards. Still I didn’t break pace, this was my town after all. The vehicle was an old white sedan rusted at the edges with a few dents on the hood and one chrome antennae standing tall. A plastic crucifix hung from the rearview mirror. Behind the wheel, a woman with long gray hair and a wrinkled face looked at me wide-eyed and wary. A glowing cig hung from her thin lips. I pressed pause.

“You need to take that off n’ burn it, compadre,” she said in a menacing, raspy voice, taking a hit and exhaling a heavy cloud of smoke. Her left elbow rested on the window frame and the flickering cigarette, now at the tip of her fingers, pointed straight at my chest. The smell of cheap tobacco hung in-between us. I rolled my eyes and smirked, realizing the reason why this woman was pestering me. It was my shirt. I was wearing a vintage Mötley Crüe Shout at the Devil 1984 T-shirt, the one with a giant inverted pentagram—the Sigil of Baphomet. A cursed garment in her eyes, no doubt. To me though, it was a cool piece of rock and roll history; a work of art in its own right. A collector’s item. It was also just a cotton shirt, one my father wore a long time ago.

“Fuck off, lady. Jesus is dead,” I replied with derision, hoping this would offend her enough to leave me alone. Instead, the crone let off a loud, mischievous cackle that made me uncomfortable; her wide grin revealed multiple cavities and stained teeth. Though unnerving, there was an unmistakable honesty to her reaction; the sort of transparency only a lunatic possesses. I turned away and resumed walking. The sounds of rattling phlegm and malice, somehow disguised as laughter, reverberated behind me. Then, silence. The night was quiet again except for the car engine’s steady growl which meant the hag wasn’t driving away. An eerie sensation crawled on my skin. I looked back defiantly to find her staring at me; her striking blue eyes gleaming on the side mirror of the old sedan. She seemed excited.

“Stupid bitch,” I thought, half annoyed. The car’s bright red brake lights blinked and began to move, slowly disappearing in the dark. I took the next available turn down a narrow road to my right, scoffing at the pathetic woman and her superstition while deciding whether to restart the song or just let it play. Unconcerned, convinced five thousand years of religion was all make-believe, pitiful stories for fools and children. Arrogant, underestimating the whims and appetite of our Creator. Alone, unaware that demons are anything but superstition; quite the opposite, they come in many different forms and their taste is…most eccentric. And thus despite the bedlam’s grim warning, I continued my stroll undeterred—a mistake I would soon pay for in flesh and blood.

Thick misgiving clouds began to take a hold of the sky; indifferent, the stars above did nothing but contemplate. Shadows crept uncontested. A string of alternative rock anthems and a few turns later, everything looked the same deep in the suburban sprawl, yet nothing seemed familiar. I found myself in an alley, walking on loose gravel when I saw it: a person. A man, sitting on a porch to my right; just far enough from the dim streetlamp to appear as nothing more than a bleak silhouette: an apparition. But he was there; the weak incandescent glow coming from inside his house betrayed him. Attempting to show traditional courtesy, I nodded and made a small hand gesture—the figure didn’t move an inch. Not good.

“Way down, mark the grave,” crooned Gerard Way in my earbuds as I strode past the man when he calmly stood up and began moving towards the road, towards the light. Moving in the dark he seemed more arachnid than human; a few crooked steps later there he was, under the streetlight, ten feet away and impossibly tall. I stood still, awkwardly staring at the specimen before me. Everything about him was just a bit too long for comfort: his limbs stretched to abnormal lengths and large tendons protruded at every joint. An excess of cartilage gave him a grotesque appearance. Acromegalia, maybe? I thought, gawking at the man in disbelief. His pale skin drew a sharp contrast with the black Metallica T-shirt he was wearing: rows of cross-shaped headstones lined up on a field—the cover art for Master of Puppets. Nice. There were dark stains on his jean shorts and he was barefoot. Wild hair. Absent eyes. Morose lips. It was hard to tell if he was even looking at me. I pressed pause and took off my buds.

“Hey I’m looking for the Leper Messiah, seen him around?” I asked, trying to lighten the mood, embarrassed to have stared for so long. No response of any kind. Not Verbal. Not Physical. Nothing. After a second of silence I turned around and resumed my stroll, feeling uneasy. I put the headphones back on but did not restart the music; the sound of my footsteps carried me to the end of the block. I took a left and couldn’t help looking back. He was on the same spot, under the same incandescent light yet something was different. Maybe it was the increased distance between us but now he seemed alert. His head was definitely tilted in my direction. Feeling even more unsettled I picked up the pace. It was time to head home.

Checking over my shoulder every few steps I reached the next street. Before making a right turn, I looked behind me one last time to make sure my sinister encounter was over. The empty road was mostly dark and the trees swayed back-and-forth to the wind’s ominous melody. No sign of him. Relieved, I pulled out my iPod and began scrolling down the long list of eyeliner bands, looking for something upbeat; the screen’s blue glow illuminated my face as I made my choice: “Chop Suey!” by System of a Down. Satisfied, I pressed play and put the Apple device back in my pocket. Looking up, my short-lived sense of safety vanished. I could see the man, barely, a block away on the opposite corner—hunched-over on the obscured sidewalk. His foreboding figure poised perfectly still. Had he been there this whole time? I was sure the street was empty just a minute ago but then again…it didn’t really matter, at that moment there was only one thought loud inside my head: this fucker is following me. I looked away and continued walking, this time a little faster. Once out of his field of vision, my stride sped-up into a run.

“Wake up!” barked Serj Tankian in my ears as I took a sharp right into an alleyway. Empty trash cans stood witness to the terror in my movements.
“Grab a brush and put a little make-up!” I made a left into an avenue and kept running; my black Converse high-tops gripped the hardened concrete at every corner.
“Hide the scars to fade away the shake-up!” The track raged untethered in-between melodic hooks and sacrilegious questions. Six more blocks in, the Campus Tower was finally visible. Slowing down and breathing hard, I took off my buds and looked around; silent rows of homes stared back at me.

I walked the next several blocks with caution: catching my breath, sticking to the sidewalk and without any music. Instead I listened to the sound of rustling leaves harmonizing alongside a choir of crickets; the ghostly call of a lone owl accentuated their tune at will. I was in the middle of an intersection when the somber, natural symphony came to an abrupt halt. For a second, there was absolute silence. Then, with a loud pop, every streetlamp around me went out: shards of glass rained down on the pavement followed by near pitch darkness. A drop of cold sweat ran down my neck. Far off in the distance, erratic footsteps moved wickedly up and down the neighborhood. It was him, he was coming for me. Afraid, trying not to panic, I flipped-open my Nokia cell phone and dialed 911.

“911 what’s your emergency?” a female voice inquired in a monotone, professional demeanor.

“Hey yeah, listen, I think I’m being stalked by some guy,” I said, checking in every direction, trying to pinpoint the threat.

“What’s your location?” the operator replied.

“Um, a few blocks west of campus,” my heartbeat accelerated, the footsteps were getting louder.

“I’m sending a patrol your way. Do you have a description of the suspect?” she asked, mechanically typing on her keyboard.

“Yeah he is tall, dark hair, pale skin, lanky and pretty fucking unsettling. He’s wearing a Metallica T-shirt a couple of sizes too small for him.” Silence. “Hello?” I ventured, feeling a knot in my stomach. The footsteps were close now.

“Run…” said the voice on the other side of the line, quiet and meek—scared. I stood there, frozen. “Run!” shrieked the emergency operator in a terrified pitch. At the same time, the approaching sound of bare skin on broken glass cut through my spine; I peered into the darkness—he was close…only two blocks away, moving towards me in a horrific manner. His four extremities were bent at exaggerated angles, hands and feet making contact with the ground at a syncopated pace. With every step, the man’s hips and shoulders shifted mechanically from side to side, disjointed. There was no energy wasted in his movements, every muscle-twitch was sharp and with purpose; a fine-tuned supernatural predator. Quick, ill-boding steps carried him viciously through the road leading straight to me. A primal fear overwhelmed every nerve cell in my body; then, a shot of adrenaline mobilized my legs and I began sprinting in the opposite direction. Prey, after all, always makes a run for it…though usually to no avail. But maybe there was hope for me, still; I just had to find the patrol car. Am I even going the right way? But there was no time to think. In a matter of seconds, I felt the man’s razor-sharp hand grip my hair from behind while pulling me down. His grip tore off a piece of my scalp as he put me hard on the ground. In an instant, his entire frame was on top of me. Arms. Knees. Elbows. Legs. Every one of my limbs pinned down. His right palm pressed so hard against my face I felt my right cheekbone crack and sink, while his heinous fingers and toes dug deep into my body at every point of contact. A pool of blood began to take shape under my head. The man looked at me with dull eyes and opened his mouth; I looked up in horror to see it was full of molars. No incisors. No canines. Only an excess of malformed bicuspids clustered on top of each other, tearing through bleeding, bright red gums.

“Help!” I screamed desperately and repeatedly into the night. There was no reply. An eerie stillness descended upon us. “What the fuck?!” was my last feeble thought before I closed my eyes in disbelief; a brutal death seemingly inevitable. Instead, warm bile, blood and meat poured down on me from his insides. The man was vomiting. He gagged, loosened his grip and retched some more. The stench of organic waste filled my nostrils. I suddenly felt his entire weight come off me and watched him heave a few feet away: arched back. Limbs twisted. A ghastly figure spewing thick fluids in the dark. Wide-eyed and in shock I felt paralyzed.

“Get up!” I thought desperately but my body didn’t respond. On his knees, suddenly the ghoul began trembling and making guttural noises. He straightened his torso and gripped the pavement with diabolic strength; his wide open mouth faced the sky. I sat there, terrified, unable to move…until I noticed a lumpy, round-shaped object the size of a soccer ball inside of him, slowly working its way up his neck. Overcoming terror and pain, using my last ounce of adrenaline, I managed to stand up and flee—slow at first, wounded. Behind me, a loud thud on the ground followed by a hollow gasp. I turned around just long enough to see that the round-shaped object, now laying on the asphalt, had a face. I looked away and kept moving as fast as my legs could take me; too scared to even scream. Drenched in filth, every pungent breath reminded me I was a dead man. Nauseated, I took off the foul T-shirt and dropped it on the callous road.

The sound of my frantic footsteps echoed on the empty street. I searched my pockets for the cell phone but it was gone; must’ve dropped it when that thing took me down. I kept running. Up ahead, red and blue lights flooded an alleyway; as I drew closer, a vehicle’s headlights poured into the street. I stepped into the light and the high beams came on, blinding me. I heard the car door open.

“Don’t move. Put your hands where I can see ‘em,” commanded a voice without hesitation.

“H-help!” I managed to blurt out, raising my hands and gasping for air; blood dripping from my mangled body.

“Dispatch I found the possible victim on 13th and Ash, I’m taking him to the ER,” he said in a hurry, turning off the lights.

“Please, we need to get the fuck out of here!” I blurted out, my eyes still readjusting to the dark. The cop proceeded to give instructions but his voice quickly faded into background noise. It was over. We were dead. The demon was here, standing still-as-a-granite statue on the sidewalk; only a few feet away from me but outside the policeman’s field of vision. Wild hair. Absent eyes. Morose lips. On his left hand, a grimy black piece of cloth hung in-between his blood-tainted fingers.

“Hey you listenin’ to me?” I felt the officer’s right hand on the back of my neck. “Lower your voice, get in the vehicle and keep your fucking head down,” he said, walking me to the car and checking over his shoulder every few steps. He was clearly agitated. I slumped in the backseat, dreading the moment those headlamps came back on, convinced the creature would be standing right in front of us, eager to kill. I expected his fiendish claws to shatter through the window, grab me by the neck and crush my windpipe at any moment. But nothing of the sort happened. The cruiser switched gears and we drove away in silence.

There was blood everywhere and my mind was in pieces. The officer had an apprehensive demeanor about him; the sort of look one has when you haven’t been on the job long enough. Neither of us said a word. A large Sonic drink sat in one of the cup holders; in the other, a crumpled AllSup’s burrito wrapper covered in grease. Scattered shotgun shells wavered nervously on the passenger seat. The policeman adjusted his rearview mirror in my direction.

“You ok back there, amigo?” he asked, lowering the window an inch and covering his nose. My stink was suffocating. I remained silent. The officer pressed several keys on his Mobile Data Terminal, a John Wayne bobblehead stood smug on the dashboard. Sirens blaring, a couple of patrol cars sped past us in the opposite direction. The back of my head felt wet.

“They won’t find him,” he said, almost annoyed, rubbing his left shoulder. “But maybe that’s a good thing.” I looked up at his reflection for the first time. He noticed. The officer shifted his weight on the seat and hesitated; after a short pause, he continued in a serious tone. “Whatever that thing is attacked you tonight, it shows up at random in boondocks all across the Great Plains. Always during this season. Always in the dark. Any townie sum bitch from here to the Mississippi can tell you about it.” He took a sip through the red, plastic straw and kept going. “Hell, the locals even have names for it depending on where you’re at. Ever heard of The Collector or Metalhead? Every podunk around calls him somethin’ different; it’s like any other urban legend, you know how it goes…this one just happens to be true.”
There was a hint of pride in that last remark. The faint radio transmission’s steady buzz, occasionally interrupted by law enforcement code, played in the background to the officer’s words.

“Every now and again, relatively speaking mind you, we get reports of a sightin’: ‘there is a creepy tall man at the end of the street’ or ‘a long freak is lurkin’ in the alley’, you know, that sort of thing. It’s fine most of the time but every now and again it doesn’t end well—old people know better than most.” He gripped the steering wheel with both hands. I leaned back and took a deep breath, my whole body was beginning to throb. The officer turned on the blinker and continued, unabashed. “My own uncle saw him up close for a second back in ‘86: he was driving home from work late one night when out of nowhere a towering pale man, standing in the middle of the road, forced him to slam on the brakes. The truck’s headlights landed on the thing for a split second; then, it was gone. My uncle swears this guy was wearing an Iron Maiden T-shirt, the one where Satan is hanging on puppet strings—The Number of the Beast,” he said raising his voice, it was apparent the officer was trying to keep me awake. A neon red cross appeared on the horizon.

“You’re not the first, you know? Survivor, I mean. At least not according to the stories we hear from neighboring police departments. The way I see it, Metalhead likes to play with his food,” the policeman asserted. “Poor bastards, apparently they never en…” he stopped himself before finishing the sentence, a funereal silence filled the space. If the rumors he’d heard were true, a cruel fate awaited me. Exhausted, I placed my forehead on the steel mesh-cage between us and started to sob quietly. Lighting up, the iPod’s LED in my pocket managed to highlight the dark stains on my jeans. Blood, just starting to coagulate, dripped sluggishly down my naked chest. I lost consciousness in the backseat of the patrol car. Humiliated. Broken. Defeated.

That was the last summer I ever spent in the Faustian town, its bloodied streets belong to the devil alone; starving ghosts roam the sullen roads, feeding on the anguish of his victims…feeding on me. I can feel them every day, a heavy sense of dread resting cold on my head; painful at times, like wearing a crown of lead. Soon after being released from the hospital, once the numerous stitches on my body had some time to heal, I packed my suitcase and left the cursed, derelict place—never to return. One last picturesque sunrise, full of color and fiction, witnessed my departure at dawn.

Today, fifteen years later, I live covered in scars: a daily reminder of my harrowing escape and the terrifying possibility of undiscovered horrors. Most people can’t understand how everything changes, once you’ve seen a demon dead-in-the-eyes…if something like Metalhead can exist in the flesh, who’s to say there aren’t a thousand other creatures of nightmare? Legions, even: supernatural, capricious lifeforms with evil-intent and sinister appetite, ripping apart unsuspecting humans in the darkest corners of the Earth. And so I spend my days in fear and self-imposed isolation; only going out when absolutely necessary and never past sundown. Just to be safe. There are six deadbolt locks on my front door, each with its own stainless steel security chain. Aluminum roller shutters reinforce the plexiglass windows of my small apartment. I’ve been working with different construction crews in the big city since, only ever taking daytime jobs though. Just to be safe. I never finished my degree. After work, most of my free time is spent in front of the computer screen, looking for signs of him or his kin. Though things may seem normal on the surface, macabre tales of unexplained disappearances and violent ends abound…if you know where to look. Whispers of “the pale man” pervade certain online forums; however, actual sightings are scarce. One single blurry picture evidences his existence: perched on top of a streetlight, his unmistakable ghoulish appearance fixed on the person holding the camera. A living gargoyle. Apparently the JPEG was found and leaked a few years back by a rookie working the Evidence Locker at the police department. The sorry son-of-a-bitch who took the photo remains unidentified, his head was never recovered.

Sometimes I lay awake at night, unable to sleep; wondering if the sound of looming, ever-approaching footsteps is coming from outside the door or inside my head. Paralyzed, afraid of shadows and dust. Certainly, once you begin to doubt your own thoughts, madness becomes inevitable; sanity gets peeled away like a scab, leaving behind a festering psychosis. Damaged. Irreparable. Shattered pieces of what once was a man. Beware, evil exists. Alone in my room, surrounded by empty fast-food containers and a meticulous variety of religious iconography; I grit my teeth and curse my luck that I ever walked the desolate, haunted roads of that small college town. Marked for life. A sacrifice, plaything for the damned. Another martyr to its beautiful, bedeviled summer nights.
",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
            };

            var poems = new List<Thread>
            {
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "No Man Is An Island",
                    Content = @"
No man is an island,
Entire of itself, 
Every man is a piece of the continent,                            
A part of the main.                             
If a clod be washed away by the sea,                            
Europe is the less.                          
As well as if a promontory were.                       
As well as if a manor of thy friend’s                            
Or of thine own were:                             
Any man’s death diminishes me,                           
Because I am involved in mankind,                   
And therefore never send to know for whom the bell tolls;                             
It tolls for thee.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Still I Rise",
                    Content = @"
You may write me down in history
With your bitter, twisted lies,
You may tread me in the very dirt
But still, like dust, I’ll rise.
Does my sassiness upset you?
Why are you beset with gloom?
’Cause I walk like I’ve got oil wells
Pumping in my living room.
Just like moons and like suns,
With the certainty of tides,
Just like hopes springing high,
Still I’ll rise.
Did you want to see me broken?
Bowed head and lowered eyes?
Shoulders falling down like teardrops.
Weakened by my soulful cries.
Does my haughtiness offend you?
Don’t you take it awful hard
’Cause I laugh like I’ve got gold mines
Diggin’ in my own back yard.
You may shoot me with your words,
You may cut me with your eyes,
You may kill me with your hatefulness,
But still, like air, I’ll rise.
Does my sexiness upset you?
Does it come as a surprise
That I dance like I’ve got diamonds
At the meeting of my thighs?
Out of the huts of history’s shame
I rise
Up from a past that’s rooted in pain
I rise
I’m a black ocean, leaping and wide,
Welling and swelling I bear in the tide.
Leaving behind nights of terror and fear
I rise
Into a daybreak that’s wondrously clear
I rise
Bringing the gifts that my ancestors gave,
I am the dream and the hope of the slave.
I rise
I rise
I rise.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Stopping By Woods On A Snowy Evening",
                    Content = @"
Whose woods these are I think I know.
His house is in the village though;
He will not see me stopping here
To watch his woods fill up with snow.
My little horse must think it queer
To stop without a farmhouse near
Between the woods and frozen lake
The darkest evening of the year.
He gives his harness bells a shake
To ask if there is some mistake.
The only other sound’s the sweep
Of easy wind and downy flake.
The woods are lovely, dark and deep,
But I have promises to keep,
And miles to go before I sleep,
And miles to go before I sleep.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Shall I Compare Thee To A Summer Day ?",
                    Content = @"
Shall I compare thee to a summer’s day?
Thou art more lovely and more temperate.
Rough winds do shake the darling buds of May,
And summer’s lease hath all too short a date.
Sometime too hot the eye of heaven shines,
And often is his gold complexion dimmed;
And every fair from fair sometime declines,
By chance, or nature’s changing course, untrimmed;
But thy eternal summer shall not fade,
Nor lose possession of that fair thou ow’st,
Nor shall death brag thou wand’rest in his shade,
When in eternal lines to Time thou grow’st.
So long as men can breathe, or eyes can see,
So long lives this, and this gives life to thee.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "There Will Come Soft Rain",
                    Content = @"
There will come soft rain and the smell of the ground,
And swallows circling with their shimmering sound;
And frogs in the pools singing at night,
And wild plum trees in tremulous white;
Robins will wear their feathery fire,
Whistling their whims on a low fence-wire;
And not one will know of the war, not one
Will care at last when it is done.
Not one would mind, neither bird nor tree,
If mankind perished utterly;
And Spring herself, when she woke at dawn
Would scarcely know that we were gone.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "If You Forget Me",
                    Content = @"
I want you to know
one thing.
You know how this is:
if I look
at the crystal moon, at the red branch
of the slow autumn at my window,
if I touch
near the fire
the impalpable ash
or the wrinkled body of the log,
everything carries me to you,
as if everything that exists,
aromas, light, metals,
were little boats
that sail
toward those isles of yours that wait for me.
Well, now,
if little by little you stop loving me
I shall stop loving you little by little.
If suddenly
you forget me
do not look for me,
for I shall already have forgotten you.
If you think it long and mad,
the wind of banners
that passes through my life,
and you decide
to leave me at the shore
of the heart where I have roots,
remember
that on that day,
at that hour,
I shall lift my arms
and my roots will set off
to seek another land.
But
if each day,
each hour,
you feel that you are destined for me
with implacable sweetness,
if each day a flower
climbs up to your lips to seek me,
ah my love, ah my own,
in me all that fire is repeated,
in me nothing is extinguished or forgotten,
my love feeds on your love, beloved,
and as long as you live it will be in your arms
without leaving mine.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "O Captain! My Captain!",
                    Content = @"
O Captain! my Captain! our fearful trip is done;
The ship has weather’d every rack, the prize we sought is won;
The port is near, the bells I hear, the people all exulting,
While follow eyes the steady keel, the vessel grim and daring:
But O heart! heart! heart!
O the bleeding drops of red,
Where on the deck my Captain lies,
Fallen cold and dead.
O Captain! my Captain! rise up and hear the bells;
Rise up — for you the flag is flung — for you the bugle trills;
For you bouquets and ribbon’d wreaths — for you the shores a-crowding;
For you they call, the swaying mass, their eager faces turning;
Here Captain! dear father!
This arm beneath your head;
It is some dream that on the deck,
You’ve fallen cold and dead.
My Captain does not answer, his lips are pale and still;
My father does not feel my arm, he has no pulse nor will;
The ship is anchor’d safe and sound, its voyage closed and done;
From fearful trip, the victor ship, comes in with object won; 20
Exult, O shores, and ring, O bells!
But I, with mournful tread,
Walk the deck my Cptain lies,
Fallen cold and dead.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Fire And Ice",
                    Content = @"
Some say the world will end in fire,
Some say in ice.
From what I’ve tasted of desire
I hold with those who favor fire.
But if it had to perish twice,
I think I know enough of hate
To say that for destruction ice
Is also great
And would suffice.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "The Road Not Taken",
                    Content = @"
Two roads diverged in a yellow wood,
And sorry I could not travel both
And be one traveler, long I stood
And looked down one as far as I could
To where it bent in the undergrowth;
Then took the other, as just as fair,
And having perhaps the better claim
Because it was grassy and wanted wear,
Though as for that the passing there
Had worn them really about the same,
And both that morning equally lay
In leaves no step had trodden black.
Oh, I kept the first for another day!
Yet knowing how way leads on to way
I doubted if I should ever come back.
I shall be telling this with a sigh
Somewhere ages and ages hence:
Two roads diverged in a wood, and I,
I took the one less traveled by,
And that has made all the difference.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Dreams",
                    Content = @"
Hold fast to dreams
For if dreams die
Life is a broken-winged bird
That cannot fly.
Hold fast to dreams
For when dreams go
Life is a barren field
Frozen with snow.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Trees",
                    Content = @"
I think that I shall never see
A poem lovely as a tree.
A tree whose hungry mouth is prest
Against the earth’s sweet flowing breast;
A tree that looks at God all day,
And lifts her leafy arms to pray;
A tree that may in summer wear
A nest of robins in her hair;
Upon whose bosom snow has lain;
Who intimately lives with rain.
Poems are made by fools like me,
But only God can make a tree.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Ozymandias",
                    Content = @"
I met a traveller from an antique land
Who said: `Two vast and trunkless legs of stone
Stand in the desert. Near them, on the sand,
Half sunk, a shattered visage lies, whose frown,
And wrinkled lip, and sneer of cold command,
Tell that its sculptor well those passions read
Which yet survive, stamped on these lifeless things,
The hand that mocked them and the heart that fed.
And on the pedestal these words appear —
“My name is Ozymandias, king of kings:
Look on my works, ye Mighty, and despair!”
Nothing beside remains. Round the decay
Of that colossal wreck, boundless and bare
The lone and level sands stretch far away.’",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Love After Love",
                    Content = @"
The time will come
when, with elation
you will greet yourself arriving
at your own door, in your own mirror
and each will smile at the other’s welcome,
and say, sit here. Eat.
You will love again the stranger who was your self.
Give wine. Give bread. Give back your heart
to itself, to the stranger who has loved you
all your life, whom you ignored
for another, who knows you by heart.
Take down the love letters from the bookshelf,
the photographs, the desperate notes,
peel your own image from the mirror.
Sit. Feast on your life.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Remember",
                    Content = @"
Remember me when I am gone away,
Gone far away into the silent land;
When you can no more hold me by the hand,
Nor I half turn to go yet turning stay.
Remember me when no more day by day
You tell me of our future that you plann’d:
Only remember me; you understand
It will be late to counsel then or pray.
Yet if you should forget me for a while
And afterwards remember, do not grieve:
For if the darkness and corruption leave
A vestige of the thoughts that once I had,
Better by far you should forget and smile
Than that you should remember and be sad.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "A Fairy Song",
                    Content = @"
Over hill, over dale,
Thorough bush, thorough brier,
Over park, over pale,
Thorough flood, thorough fire!
I do wander everywhere,
Swifter than the moon’s sphere;
And I serve the Fairy Queen,
To dew her orbs upon the green;
The cowslips tall her pensioners be;
In their gold coats spots you see;
Those be rubies, fairy favours;
In those freckles live their savours;
I must go seek some dewdrops here,
And hang a pearl in every cowslip’s ear.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Do Not Stand At My Grave And Weep",
                    Content = @"
Do not stand at my grave and weep
I am not there. I do not sleep.
I am a thousand winds that blow.
I am the diamond glints on snow.
I am the sunlight on ripened grain.
I am the gentle autumn rain.
When you awaken in the morning’s hush
I am the swift uplifting rush
Of quiet birds in circled flight.
I am the soft stars that shine at night.
Do not stand at my grave and cry;
I am not there. I did not die.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
            };

            var genreThreads = new List<GenreThread>();

            foreach (var thread in stories)
            {
                int num = rnd.Next(1, 4);
                thread.AuthorId = data.Users.OrderBy(u => Guid.NewGuid()).FirstOrDefault().Id;
                for (int i = 0; i < num; i++)
                {
                    int genreId = data.Genres
                        .Where(g => g.GenreTypeId == 1)
                        .OrderBy(g => Guid.NewGuid())
                        .FirstOrDefault().Id;
                    if(!genreThreads.Any(g => g.GenreId == genreId && g.ThreadId == thread.Id))
                    {
                        genreThreads.Add(new GenreThread
                        {
                            GenreId = genreId,
                            ThreadId = thread.Id
                        });
                    }
                }
            }

            foreach (var thread in poems)
            {
                int num = rnd.Next(1, 4);
                thread.AuthorId = data.Users.OrderBy(u => Guid.NewGuid()).FirstOrDefault().Id;
                for (int i = 0; i < num; i++)
                {
                    int genreId = data.Genres
                        .Where(g => g.GenreTypeId == 2)
                        .OrderBy(g => Guid.NewGuid())
                        .FirstOrDefault().Id;
                    if (!genreThreads.Any(g => g.GenreId == genreId && g.ThreadId == thread.Id))
                    {
                        genreThreads.Add(new GenreThread
                        {
                            GenreId = genreId,
                            ThreadId = thread.Id
                        });
                    }
                }
            }

            data.Threads.AddRange(stories);
            data.Threads.AddRange(poems);
            data.GenreThreads.AddRange(genreThreads);
            data.SaveChanges();
        }

        private static void SeedComments(_4draftsDbContext data)
        {
            if (data.Comments.Any(c => c.Id == "20bf1ac5-1a97-4854-a9ec-467f06a82fb9")) return;

            Random rnd = new Random();
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = "20bf1ac5-1a97-4854-a9ec-467f06a82fb9",
                    Content = "Amazing, I absolutely love it",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    ThreadId = "f3cd7787-ead6-4eae-899c-e5952e4934c1"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Great, wish there was more",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "eb6d0403-1b52-454a-a3bc-bc8aa534c4a1",
                    ThreadId = "b2f87ea3-676e-4e8c-9878-908a24ee4354"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Very interesting",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    ThreadId = "57cf58c3-44a5-4a32-a83a-49b36d8bf7c6"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "that one was goooooooooooood",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    ThreadId = "f1767a58-5c40-4a1d-b256-bb96fa60d9ea"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "love the twist at the end",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    ThreadId = "57cf58c3-44a5-4a32-a83a-49b36d8bf7c6"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "is there a part 2 by any chance ?????",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    ThreadId = "e78055f5-d6da-4ab4-a05d-000a3c5c5281"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "kinda drags out at the end tbh, but interesting nevertheless",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    ThreadId = "aa5067eb-6029-48f3-968a-6ab92d66b953"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "I didn't get it",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f76ba675-da23-45ae-b351-7854af84d238",
                    ThreadId = "651c10e9-6439-479e-8fad-c5784ecff65b"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "the main character is so boring",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f76ba675-da23-45ae-b351-7854af84d238",
                    ThreadId = "937e8c3d-5474-4e32-b019-cbe628bba891"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "what is it about white people trying to hunt ghosts",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f76ba675-da23-45ae-b351-7854af84d238",
                    ThreadId = "0760686a-6598-43e6-9ed7-87f340c78d7c"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "I didn't get it",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    ThreadId = "fe016357-389e-4d3f-b335-1e3a9f17ffb3"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "the main character is so boring",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    ThreadId = "fe8b0fc3-174f-4270-8e77-de2c00e6c47c"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "what is it about white people trying to hunt ghosts",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    ThreadId = "ffa37ac4-575a-4b06-b4b9-23365c62d15c"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Everyone was busy, so I went to the movie alone.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "742466ec-5456-4f41-8b02-5ca6c710fa76",
                    ThreadId = "f3cd7787-ead6-4eae-899c-e5952e4934c1"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "The gloves protect my feet from excess work.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "742466ec-5456-4f41-8b02-5ca6c710fa76",
                    ThreadId = "f1767a58-5c40-4a1d-b256-bb96fa60d9ea"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "There are no heroes in a punk rock band.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "742466ec-5456-4f41-8b02-5ca6c710fa76",
                    ThreadId = "e98d96ea-7c4a-4228-8373-f9f6bb3a8880"
                },
                 new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "The book is in front of the table.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    ThreadId = "e8d2f57f-2832-421f-aac0-207ea9b5e3e5"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "We're careful about orange ping pong balls because people might think they're fruit.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    ThreadId = "e98d96ea-7c4a-4228-8373-f9f6bb3a8880"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Eating eggs on Thursday for choir practice was recommended.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    ThreadId = "e8d2f57f-2832-421f-aac0-207ea9b5e3e5"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Jerry liked to look at paintings while eating garlic ice cream.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    ThreadId = "e78055f5-d6da-4ab4-a05d-000a3c5c5281"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "I thought red would have felt warmer in summer but I didn't think about the equator.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    ThreadId = "e6409d32-32ff-4186-9c3f-d63523794eae"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Last Friday I saw a spotted striped blue worm shake hands with a legless lizard.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    ThreadId = "df049477-da1e-4a0c-a779-729db036e4d9"
                },
                 new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "She did her best to help him.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    ThreadId = "b2f87ea3-676e-4e8c-9878-908a24ee4354"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "He had reached the point where he was paranoid about being paranoid.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    ThreadId = "b0194a3c-711b-4aff-93f1-8ecaaef7d389"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "She hadn't had her cup of coffee, and that made things all the worse.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    ThreadId = "aa5067eb-6029-48f3-968a-6ab92d66b953"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Toddlers feeding raccoons surprised even the seasoned park ranger.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "68091adf-6141-48d9-8374-4693f21c6882",
                    ThreadId = "e78055f5-d6da-4ab4-a05d-000a3c5c5281"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "The Guinea fowl flies through the air with all the grace of a turtle.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "68091adf-6141-48d9-8374-4693f21c6882",
                    ThreadId = "e6409d32-32ff-4186-9c3f-d63523794eae"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "It was the first time he had ever seen someone cook dinner on an elephant.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "68091adf-6141-48d9-8374-4693f21c6882",
                    ThreadId = "df049477-da1e-4a0c-a779-729db036e4d9"
                },
                 new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "Thigh-high in the water, the fisherman’s hope for dinner soon turned to despair.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    ThreadId = "b2f87ea3-676e-4e8c-9878-908a24ee4354"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "The irony of the situation wasn't lost on anyone in the room.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    ThreadId = "b0194a3c-711b-4aff-93f1-8ecaaef7d389"
                },
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "A dead duck doesn't fly backward.",
                    CreatedOn = RandomDayFunc(),
                    Points = rnd.Next(100),
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    ThreadId = "aa5067eb-6029-48f3-968a-6ab92d66b953"
                },

            };

            data.Comments.AddRange(comments);
            data.SaveChanges();
        }

        private static DateTime RandomDayFunc()
        {
            DateTime start = new DateTime(2018, 1, 1);
            Random gen = new Random();
            int range = ((TimeSpan)(DateTime.Today - start)).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}
