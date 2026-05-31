START TRANSACTION;
CREATE TABLE "CompanyRatings" (
    "CompanyId" uuid NOT NULL,
    "AverageRating" numeric(3,2) NOT NULL,
    "TotalReviews" integer NOT NULL,
    "RatingDistribution" text NOT NULL,
    "LastUpdated" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_CompanyRatings" PRIMARY KEY ("CompanyId"),
    CONSTRAINT "FK_CompanyRatings_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CompanyReviews" (
    "Id" uuid NOT NULL,
    "CompanyId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Rating" integer NOT NULL,
    "Title" text NOT NULL,
    "ReviewText" text NOT NULL,
    "JobTitle" text,
    "InterviewExperience" integer NOT NULL,
    "SalaryOffered" numeric,
    "HiringProcessDuration" integer,
    "CompanyResponse" text,
    "HelpfulCount" integer NOT NULL,
    "IsVerified" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_CompanyReviews" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CompanyReviews_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyReviews_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CompanyReviewHelpfuls" (
    "ReviewId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_CompanyReviewHelpfuls" PRIMARY KEY ("ReviewId", "UserId"),
    CONSTRAINT "FK_CompanyReviewHelpfuls_CompanyReviews_ReviewId" FOREIGN KEY ("ReviewId") REFERENCES "CompanyReviews" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyReviewHelpfuls_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_CompanyReviewHelpfuls_UserId" ON "CompanyReviewHelpfuls" ("UserId");

CREATE INDEX "IX_CompanyReviews_CompanyId_CreatedAt" ON "CompanyReviews" ("CompanyId", "CreatedAt");

CREATE UNIQUE INDEX "IX_CompanyReviews_UserId_CompanyId" ON "CompanyReviews" ("UserId", "CompanyId");

CREATE INDEX "IX_CompanyReviews_UserId_CreatedAt" ON "CompanyReviews" ("UserId", "CreatedAt");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260522193207_AddCompanyReviews', '9.0.12');

COMMIT;

