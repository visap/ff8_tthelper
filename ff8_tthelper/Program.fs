﻿open System.Drawing

let imageDir = System.IO.Directory.GetCurrentDirectory() + @"\..\..\images\"

let opponentHandCardYOffsets = [0 ; 154 ; 308; 462 ; 616]
let myHandCardYOffsets = [0 ; 154 ; 309; 463 ; 617]
let cardSelectionOffset = Size(-45, 0)
let (fieldCardXOffset, fieldCardYOffset) = (0, 0)

let topDigitOffset = Size(15, 0)
let leftDigitOffset = Size(0, 39)
let rightDigitOffset = Size(30, 39)
let bottomDigitOffset = Size(15, 78)

let opponentCards = [| for i in 0..4 -> Point(331, 94) + Size(0, opponentHandCardYOffsets.[i]) |]
let myCards = [| for i in 0..4 -> Point(1381, 93) + Size(0, myHandCardYOffsets.[i]) |]
let fieldCards = array2D [ for i in 0..2 -> [ for j in 0..2 -> Point(616 + fieldCardXOffset*i, 93 + fieldCardYOffset*j) ] ]

let mutable digitNames: list<string> = []

// TODO: Variant that is a bit more likely to return true for middle pixels?
let isWhitish(color: Color) = color.GetBrightness() > 0.45f && color.GetSaturation() < 0.1f

let getDigitFromBitmap(point: Point, img: Bitmap): Bitmap =
    let (width, height) = (26, 36)
    let subImage = new Bitmap(width, height, img.PixelFormat)
    for y = 0 to height-1 do
        for x = 0 to width-1 do
            let pixel = img.GetPixel(point.X + x, point.Y + y)
            if isWhitish(pixel) then
                subImage.SetPixel(x, y, pixel)
        done
    done
    subImage

let saveDigitFileFromScreenshot(digitName: string, point: Point, screenshot: Bitmap) =
    let digitBitmap = getDigitFromBitmap(point, screenshot)
    digitBitmap.Save(imageDir + "digit"+digitName+".png", Imaging.ImageFormat.Png)
    digitBitmap.Dispose()
    digitNames <- digitNames @ [digitName]

let saveDigitFilesFromExampleScreenshot() =
    let screenshot = new Bitmap(imageDir + "example_screenshot.jpg")

    let myCard0Selected = myCards.[0] + cardSelectionOffset

    saveDigitFileFromScreenshot("1", myCards.[1] + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("1_2", myCards.[3] + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("1_3", fieldCards.[0, 0] + topDigitOffset, screenshot)

    saveDigitFileFromScreenshot("2", myCard0Selected + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("2_2", myCards.[2] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("2_3", opponentCards.[1] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("2_4", opponentCards.[2] + topDigitOffset, screenshot)

    saveDigitFileFromScreenshot("3", opponentCards.[2] + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("3_2", opponentCards.[4] + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("3_3", opponentCards.[4] + bottomDigitOffset, screenshot)

    saveDigitFileFromScreenshot("4", myCards.[4] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("4_2", fieldCards.[0, 0] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("4_3", opponentCards.[1] + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("4_4", opponentCards.[3] + bottomDigitOffset, screenshot)

    saveDigitFileFromScreenshot("5", myCard0Selected + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("5_2", myCards.[1] + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("5_3", myCards.[4] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("5_4", opponentCards.[3] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("5_5", opponentCards.[3] + rightDigitOffset, screenshot)

    saveDigitFileFromScreenshot("6", myCards.[2] + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("6_2", fieldCards.[0, 0] + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("6_3", opponentCards.[1] + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("6_4", opponentCards.[2] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("6_5", opponentCards.[3] + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("6_6", opponentCards.[4] + leftDigitOffset, screenshot)

    saveDigitFileFromScreenshot("7", myCards.[3] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("7_2", myCards.[3] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("7_3", fieldCards.[0, 0] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("7_4", opponentCards.[1] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("7_5", opponentCards.[2] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("7_6", opponentCards.[4] + rightDigitOffset, screenshot)

    saveDigitFileFromScreenshot("8", myCards.[2] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("8_2", myCards.[3] + rightDigitOffset, screenshot)
    saveDigitFileFromScreenshot("8_3", myCards.[4] + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("8_3", myCards.[4] + rightDigitOffset, screenshot)

    saveDigitFileFromScreenshot("9", myCard0Selected + topDigitOffset, screenshot)
    saveDigitFileFromScreenshot("9_2", myCard0Selected + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("9_3", myCards.[1] + leftDigitOffset, screenshot)
    saveDigitFileFromScreenshot("9_4", myCards.[1] + bottomDigitOffset, screenshot)
    saveDigitFileFromScreenshot("9_5", myCards.[2] + topDigitOffset, screenshot)

    screenshot.Dispose()

let pixelAbsDiff(pixel1: Color, pixel2: Color): int =
    abs((int)pixel2.R - (int)pixel1.R) +
        abs((int)pixel2.G - (int)pixel1.G) +
        abs((int)pixel2.B - (int)pixel2.B)

let bitmapDifference(bitmap1: Bitmap, bitmap2: Bitmap): float =
    // TODO: Variant that emphasizes middle pixels over edge pixels?
    let maxAbsDifference = bitmap1.Width * bitmap1.Height * 255 * 3
    let pixelCoords = seq { for y in 0..(bitmap1.Height-1) do
                                for x in 0..(bitmap1.Width-1) do
                                    yield (x,y) }
    let absDifference = pixelCoords
                            |> Seq.map (fun (x,y) -> (bitmap1.GetPixel(x,y), bitmap2.GetPixel(x,y)))
                            |> Seq.sumBy pixelAbsDiff

    (float)absDifference / (float)maxAbsDifference

let printDiffs() =
    for i in 0 .. digitNames.Length-1 do
        for j in i+1 .. digitNames.Length-1 do
            let (n1, n2) = (List.nth digitNames i, List.nth digitNames j)
            let diff = bitmapDifference(new Bitmap(imageDir + "digit"+n1+".png"), new Bitmap(imageDir + "digit"+n2+".png"))
            printfn "DIFFERENCE B/W %s & %s: %f" n1 n2 diff
        done
        printfn ""
    done


[<EntryPoint>]
let main argv = 
    let sw = new System.Diagnostics.Stopwatch()
    sw.Start()

    saveDigitFilesFromExampleScreenshot()
    printDiffs()

    sw.Stop()
    printfn "Time elapsed: %A" sw.Elapsed

    0