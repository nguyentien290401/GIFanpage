create database GIFanpage
go

use GIFanpage
go

create table News(
NewsID int primary key identity(1,1),
NewsTitle nvarchar(max),
NewsDescription nvarchar(max),
NewsImage nvarchar(max),
NewsContent nvarchar(max),
CreateDate datetime);
go

create table Characters(
CharacterID int primary key identity(1,1),
CharacterName nvarchar(20),
CharacterVision nvarchar(10),
CharacterDescription nvarchar(max),
CharacterRarity nvarchar(10),
CharacterRegion nvarchar(20),
CharacterBirthday datetime,
CharacterImageCard nvarchar(max),
CharacterImageOriginal nvarchar(max)
);
go

create table Categories(
CategoryID int primary key identity(1,1),
CategoryName nvarchar(max),
CategoryDescription nvarchar(max))
go

create table Playstyles(
PlaystyleID int primary key identity(1,1),
PlaystyleName nvarchar(max))
go

create table Roles(
RoleID int primary key identity(1,1),
RoleName nvarchar(max))
go

create table Users(
UserID int primary key identity(1,1),
Name nvarchar(max),
Email nvarchar(max),
PasswordHash nvarchar(max),
UserImg nvarchar(max),
PlaystyleID int references Playstyles(PlaystyleID) on delete cascade,
RoleID int references Roles(RoleID) on delete cascade)
go

create table Feedbacks (
FeedbackID int  primary key identity(1,1),
FeedbackTitle nvarchar(max),
FeedbackContent nvarchar(max),
UserID int references Users(UserID) on delete cascade
)
go

--create table Submissions(
--SubmissionID int primary key identity(1,1),
--SubmissionName nvarchar(max),
--SubmissionDescription nvarchar(max),
--CloseDate datetime,
--FinalDate datetime,
--FeedbackTitle int references Feedbacks(FeedbackTitle) on delete cascade
--)
--go



create table Asks(
AskID int primary key identity(1,1),
Title nvarchar(max),
Description nvarchar(max),
Content nvarchar(max),
CreateDate datetime,
ViewCount int,
CommentCount int,
FileName nvarchar(max),
FilePath nvarchar(max),
IsTrue bit default(0),
UserID int references Users(UserID) on delete cascade,
CategoryID int references Categories(CategoryID) on delete cascade)
--SubmissionID int references Submissions(SubmissionID))
go

create table Comments(
CommentID int primary key identity(1,1),
Content nvarchar(max),
CommentDate datetime,
VotesCount int,
CurrentUserVoteType int,
SubCommentCount int,
AskID int references Asks(AskID)on delete cascade,
UserID int references Users(UserID),
IsTrue bit default(0),
IsHidden bit default(0))
go

create table SubComments(
SubCommentID int primary key identity(1,1),
Content nvarchar(max),
SubCommentDate datetime,
CommentID int references Comments(CommentID) on delete cascade,
UserID int references Users(UserID))
go

create table Votes(
VoteID int primary key identity(1,1),
UserID int references Users(UserID),
CommentID int references Comments(CommentID) on delete cascade,
VoteValue int)
go



use GIFanpage
go

insert into Playstyles values('Gameplay')
go
insert into Playstyles values('Collection')
go
insert into Playstyles values('Story')
go
insert into Playstyles values('Adventure')
go
insert into Playstyles values('Chill')
go

insert into Roles values('Admin')
go
insert into Roles values('Staff')
go


insert into Users values('admin', 'admin@gmail.com', '123456', '~/Content/Image/Ayaka.jpg', 1, 1)
go
insert into Users values('staff', 'staff@gmail.com', '123456', '~/Content/Image/KeqingAvatar.jpg', 1, 2)
go


insert into Categories values('Character', 'Talk about Character in game')
go
insert into Categories values('Weapon', 'Talk about Weapon for character')
go
insert into Categories values('Quest', 'Talk about quests in game')
go
insert into Categories values('Spiral Abyss', 'Talk about the relevant around Spiral Abyss each version')
go

--insert into Submissions values('Your ideas about ver 2.6', 'Deadline', '3-7-2022', '3-10-2022','')
--go
--insert into Submissions values('Your ideas about ver 2.7', 'Deadline', '3-7-2022', '3-10-2022','')
--go
--insert into Submissions values('Your ideas about ver 2.8 ', 'Deadline', '3-7-2022', '3-10-2022','')
--go
--insert into Submissions values('Your ideas about ver 3.0', 'Deadline', '5-7-2022', '5-10-2022','')
--go

use GIFanpage

select * from Asks
select * from Comments
select * from SubComments