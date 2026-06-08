// "Company © 2025. All rights reserved."

namespace Sixnet.Logging
{
    public enum SixnetLogFileRollingInterval
    {
        //
        // Summary:
        //     The log file will never roll; no time period information will be appended to
        //     the log file name.
        Infinite,
        //
        // Summary:
        //     Roll every year. Filenames will have a four-digit year appended in the pattern
        //
        //
        //     yyyy
        //
        //     .
        Year,
        //
        // Summary:
        //     Roll every calendar month. Filenames will have
        //
        //     yyyyMM
        //
        //     appended.
        Month,
        //
        // Summary:
        //     Roll every day. Filenames will have
        //
        //     yyyyMMdd
        //
        //     appended.
        Day,
        //
        // Summary:
        //     Roll every hour. Filenames will have
        //
        //     yyyyMMddHH
        //
        //     appended.
        Hour,
        //
        // Summary:
        //     Roll every minute. Filenames will have
        //
        //     yyyyMMddHHmm
        //
        //     appended.
        Minute
    }
}
