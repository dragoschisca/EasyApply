-- Create CompanyProfileViews table
CREATE TABLE IF NOT EXISTS "CompanyProfileViews" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CompanyId" UUID NOT NULL REFERENCES "Companies"("Id") ON DELETE CASCADE,
    "ViewerId" UUID REFERENCES "Users"("Id") ON DELETE SET NULL,
    "ViewedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Create JobViews table
CREATE TABLE IF NOT EXISTS "JobViews" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "JobId" UUID NOT NULL REFERENCES "Jobs"("Id") ON DELETE CASCADE,
    "ViewerId" UUID REFERENCES "Users"("Id") ON DELETE SET NULL,
    "ViewedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Add indexes for performance
CREATE INDEX IF NOT EXISTS "IX_CompanyProfileViews_CompanyId" ON "CompanyProfileViews"("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_CompanyProfileViews_ViewedAt" ON "CompanyProfileViews"("ViewedAt");
CREATE INDEX IF NOT EXISTS "IX_JobViews_JobId" ON "JobViews"("JobId");
CREATE INDEX IF NOT EXISTS "IX_JobViews_ViewedAt" ON "JobViews"("ViewedAt");
