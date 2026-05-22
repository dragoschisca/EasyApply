-- Create Notifications table
CREATE TABLE IF NOT EXISTS public."Notifications" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "UserId" uuid NOT NULL REFERENCES public."Users"("Id") ON DELETE CASCADE,
    "Title" text NOT NULL,
    "Message" text NOT NULL,
    "Link" text,
    "IsRead" boolean NOT NULL DEFAULT false,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT now()
);

-- Index for UserId to speed up lookups
CREATE INDEX IF NOT EXISTS "IX_Notifications_UserId" ON public."Notifications" ("UserId");

-- Add WhyJoinUs to Companies
ALTER TABLE public."Companies" ADD COLUMN IF NOT EXISTS "WhyJoinUs" text;
