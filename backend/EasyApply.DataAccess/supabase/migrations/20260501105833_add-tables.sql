CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                                                       "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
    );

START TRANSACTION;
CREATE TABLE "Users" (
                         "Id" uuid NOT NULL,
                         "Email" text NOT NULL,
                         "PasswordHash" text NOT NULL,
                         "UserType" integer NOT NULL,
                         "IsActive" boolean NOT NULL,
                         "EmailVerified" boolean NOT NULL,
                         "CreatedAt" timestamp with time zone NOT NULL,
                         "UpdatedAt" timestamp with time zone NOT NULL,
                         CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Candidates" (
                              "Id" uuid NOT NULL,
                              "UserId" uuid NOT NULL,
                              "FirstName" text NOT NULL,
                              "LastName" text NOT NULL,
                              "Phone" text,
                              "Location" text,
                              "LinkedInUrl" text,
                              "PortfolioUrl" text,
                              "Bio" text,
                              "CreatedAt" timestamp with time zone NOT NULL,
                              "UpdatedAt" timestamp with time zone NOT NULL,
                              CONSTRAINT "PK_Candidates" PRIMARY KEY ("Id"),
                              CONSTRAINT "FK_Candidates_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Companies" (
                             "Id" uuid NOT NULL,
                             "UserId" uuid NOT NULL,
                             "CompanyName" text NOT NULL,
                             "Industry" text,
                             "CompanySize" text,
                             "Website" text,
                             "Description" text,
                             "LogoUrl" text,
                             "Location" text,
                             "SubscriptionTier" integer NOT NULL,
                             "SubscriptionExpiresAt" timestamp with time zone,
                             "CreatedAt" timestamp with time zone NOT NULL,
                             "UpdatedAt" timestamp with time zone NOT NULL,
                             CONSTRAINT "PK_Companies" PRIMARY KEY ("Id"),
                             CONSTRAINT "FK_Companies_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CVs" (
                       "Id" uuid NOT NULL,
                       "CandidateId" uuid NOT NULL,
                       "FileName" text NOT NULL,
                       "FilePath" text NOT NULL,
                       "FileSize" integer NOT NULL,
                       "ParsedContent" text,
                       "Skills" text,
                       "Experience" text,
                       "Education" text,
                       "IsPrimary" boolean NOT NULL,
                       "UploadedAt" timestamp with time zone NOT NULL,
                       CONSTRAINT "PK_CVs" PRIMARY KEY ("Id"),
                       CONSTRAINT "FK_CVs_Candidates_CandidateId" FOREIGN KEY ("CandidateId") REFERENCES "Candidates" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Jobs" (
                        "Id" uuid NOT NULL,
                        "CompanyId" uuid NOT NULL,
                        "Title" text NOT NULL,
                        "Description" text NOT NULL,
                        "Requirements" text NOT NULL,
                        "RequiredSkills" text,
                        "EmploymentType" integer NOT NULL,
                        "ExperienceLevel" integer NOT NULL,
                        "Location" text,
                        "SalaryMin" numeric,
                        "SalaryMax" numeric,
                        "IsRemote" boolean NOT NULL,
                        "IsActive" boolean NOT NULL,
                        "ViewsCount" integer NOT NULL,
                        "ApplicationsCount" integer NOT NULL,
                        "CreatedAt" timestamp with time zone NOT NULL,
                        "UpdatedAt" timestamp with time zone NOT NULL,
                        "ExpiresAt" timestamp with time zone,
                        CONSTRAINT "PK_Jobs" PRIMARY KEY ("Id"),
                        CONSTRAINT "FK_Jobs_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Applications" (
                                "Id" uuid NOT NULL,
                                "JobId" uuid NOT NULL,
                                "CandidateId" uuid NOT NULL,
                                "CVId" uuid NOT NULL,
                                "CompatibilityScore" numeric,
                                "ScoreDetails" text,
                                "Status" integer NOT NULL,
                                "AppliedAt" timestamp with time zone NOT NULL,
                                "ReviewedAt" timestamp with time zone,
                                CONSTRAINT "PK_Applications" PRIMARY KEY ("Id"),
                                CONSTRAINT "FK_Applications_CVs_CVId" FOREIGN KEY ("CVId") REFERENCES "CVs" ("Id") ON DELETE CASCADE,
                                CONSTRAINT "FK_Applications_Candidates_CandidateId" FOREIGN KEY ("CandidateId") REFERENCES "Candidates" ("Id") ON DELETE CASCADE,
                                CONSTRAINT "FK_Applications_Jobs_JobId" FOREIGN KEY ("JobId") REFERENCES "Jobs" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SavedJobs" (
                             "Id" uuid NOT NULL,
                             "CandidateId" uuid NOT NULL,
                             "JobId" uuid NOT NULL,
                             "SavedAt" timestamp with time zone NOT NULL,
                             CONSTRAINT "PK_SavedJobs" PRIMARY KEY ("Id"),
                             CONSTRAINT "FK_SavedJobs_Candidates_CandidateId" FOREIGN KEY ("CandidateId") REFERENCES "Candidates" ("Id") ON DELETE CASCADE,
                             CONSTRAINT "FK_SavedJobs_Jobs_JobId" FOREIGN KEY ("JobId") REFERENCES "Jobs" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Applications_CandidateId" ON "Applications" ("CandidateId");

CREATE INDEX "IX_Applications_CVId" ON "Applications" ("CVId");

CREATE INDEX "IX_Applications_JobId" ON "Applications" ("JobId");

CREATE UNIQUE INDEX "IX_Candidates_UserId" ON "Candidates" ("UserId");

CREATE UNIQUE INDEX "IX_Companies_UserId" ON "Companies" ("UserId");

CREATE INDEX "IX_CVs_CandidateId" ON "CVs" ("CandidateId");

CREATE INDEX "IX_Jobs_CompanyId" ON "Jobs" ("CompanyId");

CREATE INDEX "IX_SavedJobs_CandidateId" ON "SavedJobs" ("CandidateId");

CREATE INDEX "IX_SavedJobs_JobId" ON "SavedJobs" ("JobId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260311105429_InitialCreate', '9.0.12');

ALTER TABLE "Jobs" ADD "Category" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260318094120_AddCategoryToJobs', '9.0.12');

ALTER TABLE "Jobs" ADD "Address" text;

ALTER TABLE "Candidates" ADD "GitHubUrl" text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260319081849_AddAddressAndGitHubUrl', '9.0.12');

ALTER TABLE "Jobs" DROP COLUMN "IsRemote";

ALTER TABLE "Jobs" ADD "CompanyCulture" text;

ALTER TABLE "Jobs" ADD "LocationType" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260320132253_UpdateJobLocationAndCulture', '9.0.12');

ALTER TABLE "Companies" ADD "CompanyCulture" text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260320133751_AddCompanyCultureToCompany', '9.0.12');

ALTER TABLE "Jobs" ADD "Latitude" double precision;

ALTER TABLE "Jobs" ADD "Longitude" double precision;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260320134755_AddGeolocationToJob', '9.0.12');

COMMIT;

