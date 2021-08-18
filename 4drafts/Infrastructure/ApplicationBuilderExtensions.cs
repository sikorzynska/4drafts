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

            SeedCategories(data);
            SeedUsers(data);
            SeedThreads(data);
            //SeedComments(data);

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

        private static void SeedUsers(_4draftsDbContext data)
        {
            if (data.Users.Any(u => u.Id == "68091adf-6141-48d9-8374-4693f21c6882")) return;

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
                    RegisteredOn = RandomDayFunc(),
                    PasswordHash = "AQAAAAEAACcQAAAAEKqhj3W0v2DYkh2A+hnzRAUvmzET8VyXfIgcfxyPr/zkvwcEVIMAl5UAV3P9pIL+uA==",
                    AvatarUrl = "https://cdn4.iconfinder.com/data/icons/avatars-xmas-giveaway/128/zombie_avatar_monster_dead-512.png",
                },
            };

            data.Users.AddRange(users);
            data.SaveChanges();
        }

        private static void SeedThreads(_4draftsDbContext data)
        {
            if (data.Threads.Any(t => t.Id == "8725ba9f-490d-460a-b6cf-d19d9c2ecb37")) return;
            Random rnd = new Random();

            var threads = new List<Thread>
            {
                new Thread
                {
                    Id = "fe016357-389e-4d3f-b335-1e3a9f17ffb3",
                    Title = "My sister was a sociopath. Then she had surgery.",
                    Description = "After the surgery of her sociopathic sister, Karen quickly realizes that something is clearly wrong.",
                    Content = "There was always something wrong with Annie. For years, it felt like I was the only one who knew.",
                    CategoryId = 3,
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "972026cd-3505-4f04-9ac7-382c6541ba70",
                    Title = "Incomplete",
                    Description = "A Modern Love Story by Kobra",
                    Content = "It was never about the idea of you, it was you. I never wanted a relationship, I told myself there was no way I'd ever consider marriage again before we met. But between all of the little things, the big things, the things others may never even notice...every single part that makes you who you are...I fell for it all.",
                    CategoryId = 6,
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "937e8c3d-5474-4e32-b019-cbe628bba891",
                    Title = "Days Passing",
                    Description = "When Cathy (a bored girl) decides to pretend she's missing, things start to go south quickly.",
                    Content = "Mikayla Murray went missing twelve years ago, on the eve of her 18th birthday. She didn’t have any big plans or anything, but her friends described her as having been in a particularly good mood for what was an otherwise perfectly normal Friday. She’d gone to school, soccer practice, work, and then came home for a night of movies with her kid brother, James. He was more excited for her birthday than she was. Even wanted to stay awake with her until midnight but, of course, had fallen asleep right away. When he woke in the middle of the night, he saw her headlights shining through his window and watched as they rushed down their country road, not knowing that it was the last he’d ever see of her. The poor kid was only five and would be forever tormented over why she’d left him, or why she’d never come back.",
                    CategoryId = 2,
                    AuthorId = "f3e14356-cd02-4756-baf1-93fbce922a45",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e78055f5-d6da-4ab4-a05d-000a3c5c5281",
                    Title = "Annabelle vs. the Monster Under the Bed",
                    Description = "Children's story",
                    Content = "Mommy closed the book she’d been reading and stood up, plopping Annabelle on her feet. Annabelle really wanted to hear the rest of the story, but she yawned before she could complain. Maybe Mommy was right. Annabelle started to skip down the hall to Mommy’s room before she remembered.",
                    CategoryId = 9,
                    AuthorId = "68091adf-6141-48d9-8374-4693f21c6882",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "aa5067eb-6029-48f3-968a-6ab92d66b953",
                    Title = "Kind Gestures",
                    Description = "I Offered a Homeless Man Coffee, but He Asked Me Why",
                    Content = " saw a homeless man outside of my local coffee shop, and I offered to buy him coffee. He appeared to be in his 60s, with frizzy white hair similar to what a mad scientist might don in an old black and white film. Winter was coming, and the chill air was a constant reminder of that.",
                    CategoryId = 10,
                    AuthorId = "68091adf-6141-48d9-8374-4693f21c6882",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "df049477-da1e-4a0c-a779-729db036e4d9",
                    Title = "Did You Lock The Door?",
                    Description = "",
                    Content = "Did you remember to lock the door? Before you answer that, I want you to really think about it. One, or maybe two of you, might have actually checked the locks as soon as you read the title. Maybe some of you habitually lock the door as soon as you get home. Twisting the deadbolt and hooking the chain just come as second nature. Did you notice anything out of place? Anything that could lead you to believe you aren’t alone? Would you even notice? What about the windows?",
                    CategoryId = 4,
                    AuthorId = "68091adf-6141-48d9-8374-4693f21c6882",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "0760686a-6598-43e6-9ed7-87f340c78d7c",
                    Title = "Passage",
                    Description = "What if one could go back to being 10 years old...",
                    Content = "I rubbed my eyes, but the genie was still there when I opened them again. His bulky, dark blue torso originated from a trail of smoke wafting from a broken lamp on the ground.",
                    CategoryId = 11,
                    AuthorId = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "49e9dbca-b7c0-4852-987d-73c265ee0b3f",
                    Title = "Can you see the moon?",
                    Description = "",
                    Content = "“When it is time you will see it.” He slowly looked down at her again. Her dress was almost completely red now. He felt them pressing behind his eyes, but he kept them back. There is still time, you can’t give in yet.",
                    CategoryId = 5,
                    AuthorId = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "1894e044-dd59-4000-8865-6804b13c8cd5",
                    Title = "Battered",
                    Description = "Copious amounts of twisted bottle caps",
                    Content = "This is absolutely annoying, what a not dream job whatsoever.",
                    CategoryId = 14,
                    AuthorId = "5386f041-5c28-4572-a60b-81f04ad32e61",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "b0194a3c-711b-4aff-93f1-8ecaaef7d389",
                    Title = "Christopher Walking",
                    Description = "Better than Twilight",
                    Content = "Using my newfound funding, which I later found to be not limited to man hunting, I bought a rental car, some rope, a good knife, and some other kidnapping essentials.",
                    CategoryId = 13,
                    AuthorId = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "57cf58c3-44a5-4a32-a83a-49b36d8bf7c6",
                    Title = "Better than Twilight",
                    Description = "Christopher Walking",
                    Content = "Finding the school was an easy look up, as was putting a face to the name. Their website had pictures of all their staff members, and the schedule.",
                    CategoryId = 12,
                    AuthorId = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e8d2f57f-2832-421f-aac0-207ea9b5e3e5",
                    Title = "Barrens",
                    Description = "Finding the school",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 7,
                    AuthorId = "8bcfa261-c8a2-4e83-866b-85e2649b2bde",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "ffa37ac4-575a-4b06-b4b9-23365c62d15c",
                    Title = "Kalimdor",
                    Description = "Artificial",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 8,
                    AuthorId = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "52de4f3a-7ec4-464f-a6e0-aa77c881cc53",
                    Title = "Azeroth",
                    Description = "Artificial",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 4,
                    AuthorId = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e6409d32-32ff-4186-9c3f-d63523794eae",
                    Title = "Harry Styles",
                    Description = "Artificial",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 7,
                    AuthorId = "efe1761c-030d-4329-a41a-bca51041bd2b",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "8725ba9f-490d-460a-b6cf-d19d9c2ecb37",
                    Title = "Zendaya",
                    Description = "Artificial",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 1,
                    AuthorId = "742466ec-5456-4f41-8b02-5ca6c710fa76",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "8a8e339a-8c68-4bad-9b29-c926b6aca412",
                    Title = "Title",
                    Description = "Artificial",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 1,
                    AuthorId = "742466ec-5456-4f41-8b02-5ca6c710fa76",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "331900b4-2c7e-4682-b0b8-5c5928eec238",
                    Title = "Tommy is the boy next door",
                    Description = "This is an artificially injected thread for testing purposes.",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 13,
                    AuthorId = "f76ba675-da23-45ae-b351-7854af84d238",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "6aa06fee-025d-4120-aa4e-9b6310273443",
                    Title = "Title is a title",
                    Description = "Michael Jackson",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 12,
                    AuthorId = "f76ba675-da23-45ae-b351-7854af84d238",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "651c10e9-6439-479e-8fad-c5784ecff65b",
                    Title = "Puppy on a desk",
                    Description = "Rolling Stones",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 2,
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "9fd6f1e8-ccf1-4a78-b3f1-c70bdbd2733e",
                    Title = "Rotoscope",
                    Description = "Kevin is home alone",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 3,
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e98d96ea-7c4a-4228-8373-f9f6bb3a8880",
                    Title = "Revision",
                    Description = "Raptorman",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 4,
                    AuthorId = "faf3e481-7f75-4261-bd8f-05bb00212239",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "f1767a58-5c40-4a1d-b256-bb96fa60d9ea",
                    Title = "Television",
                    Description = "Copperfield",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 5,
                    AuthorId = "6aaa7f52-73b9-4ea1-8899-efa54cac082e",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "fe8b0fc3-174f-4270-8e77-de2c00e6c47c",
                    Title = "Copperfield",
                    Description = "Television",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 6,
                    AuthorId = "6aaa7f52-73b9-4ea1-8899-efa54cac082e",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "b2f87ea3-676e-4e8c-9878-908a24ee4354",
                    Title = "Velocity",
                    Description = "Opacity",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 3,
                    AuthorId = "6aaa7f52-73b9-4ea1-8899-efa54cac082e",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "f3cd7787-ead6-4eae-899c-e5952e4934c1",
                    Title = "Opacity",
                    Description = "Velocity",
                    Content = "This is an artificially injected thread for testing purposes.",
                    CategoryId = 10,
                    AuthorId = "6aaa7f52-73b9-4ea1-8899-efa54cac082e",
                    Points = rnd.Next(200),
                    CreatedOn = RandomDayFunc(),
                },
            };

            data.Threads.AddRange(threads);
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
