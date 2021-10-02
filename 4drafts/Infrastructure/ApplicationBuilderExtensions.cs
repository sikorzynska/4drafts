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
                new ThreadType { Name = "Writing Prompt" },
                new ThreadType { Name = "Prompt Inspired Story" },
            });

            data.SaveChanges();
        }

        private static void SeedGenres(_4draftsDbContext data)
        {
            if (data.Genres.Any()) return;
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
            data.Genres.AddRange(new[]
            {
                new Genre {Name = "Literary Fiction", SimplifiedName = "L.F.", Description = descriptions["LF"] },
                new Genre {Name = "Mystery", SimplifiedName = "Myst.", Description = descriptions["Myst."]},
                new Genre {Name = "Thriller", SimplifiedName = "Thrill.", Description = descriptions["Thri."]},
                new Genre {Name = "Horror", SimplifiedName = "Horr.", Description = descriptions["Horr."]},
                new Genre {Name = "Historical", SimplifiedName = "Hist.", Description = descriptions["Hist."]},
                new Genre {Name = "Romance", SimplifiedName = "Rom.", Description = descriptions["Rom."]},
                new Genre {Name = "Western", SimplifiedName = "West.", Description = descriptions["West."]},
                new Genre {Name = "Bildungsroman", SimplifiedName = "Bild.", Description = descriptions["Bild."]},
                new Genre {Name = "Speculative Fiction", SimplifiedName = "Spec.F.", Description = descriptions["Spec. Fic."]},
                new Genre {Name = "Science Fiction", SimplifiedName = "Scie.F.", Description = descriptions["Scie. Fic."]},
                new Genre {Name = "Fantasy", SimplifiedName = "Fant.", Description = descriptions["Fant."]},
                new Genre {Name = "Dystopian", SimplifiedName = "Dyst.", Description = descriptions["Dyst."]},
                new Genre {Name = "Magical Realism", SimplifiedName = "M.R.", Description = descriptions["MR"]},
                new Genre {Name = "Realist Literature", SimplifiedName = "R.L.", Description = descriptions["RL"]},
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
                new User{
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Hoodini",
                    NormalizedUserName = "Hoodini".ToUpper(),
                    Email = "Hoodini@gmail.com".ToLower(),
                    NormalizedEmail = "zombie@gmail.com".ToUpper(),
                    FirstName = "Pendejo",
                    Youtube = "www.youtube.com",
                    Twitter = "www.twitter.com",
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

            var prompts = new List<Thread>
            {
                new Thread
                {
                    Id = "a2c45aa7-8157-4962-b4cf-545a9b551b2a",
                    ThreadTypeId = 3,
                    Content = "You enter hell. In your surprise, there is no scorching fire or gnashing of teeth. The demons are tired of torture and thought humans in hell are forsaken by the heavens too. Satan decided to build a society instead. There is one rule, whoever disrupts the order will disappear.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "07657c07-daa0-4f55-850e-383677555136",
                    ThreadTypeId = 3,
                    Content = "People who achieve great deeds are rewarded with supernatural power beyond the wildest dreams of mortal men, and apparently eating a giant burrito in under half an hour meets the criteria",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "81dd1f72-ce1e-4f53-af34-d5ccfdb00ae4",
                    ThreadTypeId = 3,
                    Content = "You were created to serve the Hero, to be their weapon, you were known as the Sword of Light, but nobody has called you that for a long time, now you're just known as the cursed sword of blightforest. You were made for the hero's endless vitality, not the ones mere mortals possess.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "64d85a5f-9ec4-41a4-bafc-618427ec2efb",
                    ThreadTypeId = 3,
                    Content = "You wake up one morning to find an email in your inbox inviting you to create an account on UsNet, a social media platform made up entirely of versions of you from alternate timelines in the multiverse.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "0ff81232-e041-424e-b2cf-2456df62c832",
                    ThreadTypeId = 3,
                    Content = "At the age of 18, everyone gains a Familiar, an animal suddenly enchanted to be intelligent and bonded to them. You wake up on your 18th birthday to find your room covered in hornets, all of them speaking to you as one.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "7e281ef6-dd28-4a41-8532-4a79907c714b",
                    ThreadTypeId = 3,
                    Content = "You are a superhero, no one knows about your alter ego. Not even your spouse. You return home tired and disappointed one day after failing to capture your archnemises. You enter your bedroom to find your spouse struggling to get out of the costume of your archnemises.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "3a85b219-ab58-4b17-84e6-d15052dcb8bb",
                    ThreadTypeId = 3,
                    Content = "Aliens have invaded to conquer and enslave humanity, however 'slavery' to them involves only working the equivalent of 12 hours a week while having healthy food, shelter, and means of entertainment taken care of so the human resistance is having trouble with defectors preferring to be slaves.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "2543d7ca-cc5c-4c29-a72a-a7771eae3209",
                    ThreadTypeId = 3,
                    Content = "Years ago, you were a feared warrior, until a witch cast a spell on you. 'May you never hurt or kill anyone by blade, word or through any other means.' Now, you are the world's greatest healer. You just open your clients and do random stuff. After all, you cannot possibly hurt them!",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e1df4d76-be16-4b56-985c-ff687ffc5682",
                    ThreadTypeId = 3,
                    Content = "You have just been abducted by a UFO. While you are figuring out what just happened to to you, a frantic alien bursts into the room. 'You have no idea how many rules I'm breaking, but my Human Studies final is tomorrow and I need help.'",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "86827847-5ea9-448f-81a4-b2ba43833226",
                    ThreadTypeId = 3,
                    Content = "You’re the god of small luck, you make the bus late, make pennies appear. You receive a prayer from a homeless man, “Please, I want to get on my feet. A stable job, a wife, some kids.” Normally, you’d forward his prayer to the god of success. Now, you decide to take on the case yourself.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
            };

            foreach (var prompt in prompts)
            {
                prompt.AuthorId = data.Users.OrderBy(u => Guid.NewGuid()).FirstOrDefault().Id;
            }

            data.Threads.AddRange(prompts);
            data.SaveChanges();

            var stories = new List<Thread>
            {
                new Thread
                {
                    Id = "fe016357-389e-4d3f-b335-1e3a9f17ffb3",
                    ThreadTypeId = 1,
                    Title = "My sister was a sociopath. Then she had surgery.",
                    Content = "There was always something wrong with Annie. For years, it felt like I was the only one who knew.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "972026cd-3505-4f04-9ac7-382c6541ba70",
                    ThreadTypeId = 1,
                    Title = "Incomplete",
                    Content = "It was never about the idea of you, it was you. I never wanted a relationship, I told myself there was no way I'd ever consider marriage again before we met. But between all of the little things, the big things, the things others may never even notice...every single part that makes you who you are...I fell for it all.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "937e8c3d-5474-4e32-b019-cbe628bba891",
                    ThreadTypeId = 1,
                    Title = "Days Passing",
                    Content = "Mikayla Murray went missing twelve years ago, on the eve of her 18th birthday. She didn’t have any big plans or anything, but her friends described her as having been in a particularly good mood for what was an otherwise perfectly normal Friday. She’d gone to school, soccer practice, work, and then came home for a night of movies with her kid brother, James. He was more excited for her birthday than she was. Even wanted to stay awake with her until midnight but, of course, had fallen asleep right away. When he woke in the middle of the night, he saw her headlights shining through his window and watched as they rushed down their country road, not knowing that it was the last he’d ever see of her. The poor kid was only five and would be forever tormented over why she’d left him, or why she’d never come back.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e78055f5-d6da-4ab4-a05d-000a3c5c5281",
                    ThreadTypeId = 1,
                    Title = "Annabelle vs. the Monster Under the Bed",
                    Content = "Mommy closed the book she’d been reading and stood up, plopping Annabelle on her feet. Annabelle really wanted to hear the rest of the story, but she yawned before she could complain. Maybe Mommy was right. Annabelle started to skip down the hall to Mommy’s room before she remembered.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "aa5067eb-6029-48f3-968a-6ab92d66b953",
                    ThreadTypeId = 1,
                    Title = "Kind Gestures",
                    Content = " saw a homeless man outside of my local coffee shop, and I offered to buy him coffee. He appeared to be in his 60s, with frizzy white hair similar to what a mad scientist might don in an old black and white film. Winter was coming, and the chill air was a constant reminder of that.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "df049477-da1e-4a0c-a779-729db036e4d9",
                    ThreadTypeId = 1,
                    Title = "Did You Lock The Door?",
                    Content = "Did you remember to lock the door? Before you answer that, I want you to really think about it. One, or maybe two of you, might have actually checked the locks as soon as you read the title. Maybe some of you habitually lock the door as soon as you get home. Twisting the deadbolt and hooking the chain just come as second nature. Did you notice anything out of place? Anything that could lead you to believe you aren’t alone? Would you even notice? What about the windows?",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "0760686a-6598-43e6-9ed7-87f340c78d7c",
                    ThreadTypeId = 1,
                    Title = "Passage",
                    Content = "I rubbed my eyes, but the genie was still there when I opened them again. His bulky, dark blue torso originated from a trail of smoke wafting from a broken lamp on the ground.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "49e9dbca-b7c0-4852-987d-73c265ee0b3f",
                    ThreadTypeId = 1,
                    Title = "Can you see the moon?",
                    Content = "“When it is time you will see it.” He slowly looked down at her again. Her dress was almost completely red now. He felt them pressing behind his eyes, but he kept them back. There is still time, you can’t give in yet.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "1894e044-dd59-4000-8865-6804b13c8cd5",
                    ThreadTypeId = 1,
                    Title = "Battered",
                    Content = "This is absolutely annoying, what a not dream job whatsoever.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "b0194a3c-711b-4aff-93f1-8ecaaef7d389",
                    ThreadTypeId = 1,
                    Title = "Christopher Walking",
                    Content = "Using my newfound funding, which I later found to be not limited to man hunting, I bought a rental car, some rope, a good knife, and some other kidnapping essentials.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "57cf58c3-44a5-4a32-a83a-49b36d8bf7c6",
                    ThreadTypeId = 1,
                    Title = "Better than Twilight",
                    Content = "Finding the school was an easy look up, as was putting a face to the name. Their website had pictures of all their staff members, and the schedule.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e8d2f57f-2832-421f-aac0-207ea9b5e3e5",
                    ThreadTypeId = 1,
                    Title = "Barrens",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "ffa37ac4-575a-4b06-b4b9-23365c62d15c",
                    ThreadTypeId = 4,
                    Title = "Kalimdor",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "52de4f3a-7ec4-464f-a6e0-aa77c881cc53",
                    ThreadTypeId = 4,
                    Title = "Azeroth",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e6409d32-32ff-4186-9c3f-d63523794eae",
                    ThreadTypeId = 4,
                    Title = "Harry Styles",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "8725ba9f-490d-460a-b6cf-d19d9c2ecb37",
                    ThreadTypeId = 4,
                    Title = "Zendaya",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "8a8e339a-8c68-4bad-9b29-c926b6aca412",
                    ThreadTypeId = 4,
                    Title = "Title",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "331900b4-2c7e-4682-b0b8-5c5928eec238",
                    ThreadTypeId = 4,
                    Title = "Tommy is the boy next door",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "6aa06fee-025d-4120-aa4e-9b6310273443",
                    ThreadTypeId = 4,
                    Title = "Title is a title",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "651c10e9-6439-479e-8fad-c5784ecff65b",
                    ThreadTypeId = 4,
                    Title = "Puppy on a desk",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "9fd6f1e8-ccf1-4a78-b3f1-c70bdbd2733e",
                    ThreadTypeId = 4,
                    Title = "Rotoscope",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "e98d96ea-7c4a-4228-8373-f9f6bb3a8880",
                    ThreadTypeId = 4,
                    Title = "Revision",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "f1767a58-5c40-4a1d-b256-bb96fa60d9ea",
                    ThreadTypeId = 4,
                    Title = "Television",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "fe8b0fc3-174f-4270-8e77-de2c00e6c47c",
                    ThreadTypeId = 4,
                    Title = "Copperfield",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "b2f87ea3-676e-4e8c-9878-908a24ee4354",
                    ThreadTypeId = 4,
                    Title = "Velocity",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(1000),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = "f3cd7787-ead6-4eae-899c-e5952e4934c1",
                    ThreadTypeId = 4,
                    Title = "Opacity",
                    Content = "This is an artificially injected thread for testing purposes.",
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
                    Title = "My sister was a sociopath. Then she had surgery.",
                    Content = "There was always something wrong with Annie. For years, it felt like I was the only one who knew.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "As well as if a manor of thy friend’s",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Or of thine own were:",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Any man’s death diminishes me,",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Because I am involved in mankind,",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "And therefore never send to know for whom the bell tolls;",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "It tolls for thee.",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                //HERE
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "Whose woods these are I think I know.",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "His house is in the village though;",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "He will not see me stopping here",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "To watch his woods fill up with snow.",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
                new Thread
                {
                    Id = Guid.NewGuid().ToString(),
                    ThreadTypeId = 2,
                    Title = "My little horse must think it queer",
                    Content = "This is an artificially injected thread for testing purposes.",
                    Points = rnd.Next(900),
                    CreatedOn = RandomDayFunc(),
                },
            };

            var genreThreads = new List<GenreThread>();

            foreach (var thread in stories)
            {
                int num = rnd.Next(1, 4);
                thread.AuthorId = data.Users.OrderBy(u => Guid.NewGuid()).FirstOrDefault().Id;
                if (thread.ThreadTypeId == 4) thread.PromptId = data.Threads.Where(t => t.ThreadTypeId == 3).OrderBy(t => Guid.NewGuid()).FirstOrDefault().Id;
                for (int i = 0; i < num; i++)
                {
                    int genreId = rnd.Next(1, 15);
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
                    int genreId = rnd.Next(1, 15);
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
