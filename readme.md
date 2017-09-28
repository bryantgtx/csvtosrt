# CSV To SRT converter

The program will convert subtitles in a comma-separated value file to subtitles in a SubRip text file.  About as basic a program as you can write.  After doing a small project in [Vegas](http://www.vegascreativesoftware.com) (using the [Vegasaur](http://vegasaur.com/) plugin), I realized three things:

1. multi-line columns in a CSV only get the first line picked up
2. my text preset doesn't autowrap in a way that I like
3. having to bounce back and forth with an online converter was slow.

This app was designed for a workflow where

1. someone creates a subtitle file in Excel or other spreadsheet
2. that spreadsheet gets exported to csv (if it didn't start that way)
3. convert the csv to srt with this program
4. import the srt with Vegasaur
5. check the results, if necessary delete the track, go back to step 3 with a different maxchar value

### Usage:
    csvToSRT source [maxchars] [target]

       source   -  csv file to read.  Expected format is start time, stop time, subtitle text
       maxchars - Optional value limiting the length of each subtitle line (useful for editors that don't wrap)
       target   - Optional value to specify the output file.  The default is <source>.srt`

Note that the max character limit will not break mid-word, nor does it provide any intelligent hyphenation, it scans backward for a decent place to put a line break.  If it doesn't find one, it scans forward for the closest it can find.


For example, the following test.csv file:

    00:00:10.00,00:00:17.00,"First remove the black strips from the sides"
    00:00:17.01,00:00:31.00,"It is easiest to start on a corner
    - trust us"
    00:00:31.13,00:00:47.00,"These strips pop in and out"

is valid, and will import, but the second line of the second subtitle was not shown, and the first line (with the font size for my preset) went beyond the edges of the frame.  

Running this through the converter will yield:


    > csvToSrt test.csv

    1
    00:00:10,000 --> 00:00:17,000
    First remove the black strips from the sides

    2
    00:00:17,001 --> 00:00:31,000
    It is easiest to start on a corner
    - trust us

    3
    00:00:31,013 --> 00:00:47,000
    These strips pop in and out


And now both lines of the second subtitle are imported correctly.  However, the first subtitle is too long.  Running it again with maxchars:


    > csvToSrt test.csv 40

    1
    00:00:10,000 --> 00:00:17,000
    First remove the black strips from the 
    sides

    2
    00:00:17,001 --> 00:00:31,000
    It is easiest to start on a corner
    - trust us

    3
    00:00:31,013 --> 00:00:47,000
    These strips pop in and out
