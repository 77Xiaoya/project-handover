$ErrorActionPreference = 'Stop'

$src = 'D:\My project (2)_copy\My project (2)_copy\FINAL_DELIVERY_DOCUMENT_LEAN.md'
$outProject = 'D:\My project (2)_copy\My project (2)_copy\FINAL_DELIVERY_DOCUMENT_POLISHED.docx'
$outDownloads = 'C:\Users\ZHANGXIAOYA\Downloads\FINAL_DELIVERY_DOCUMENT_POLISHED.docx'

function From-CodePoints([int[]]$codePoints) {
    return [string]::Concat(($codePoints | ForEach-Object { [char]$_ }))
}

function Clean-Line($line) {
    $line = $line -replace '\*\*', ''
    $line = $line -replace '`', ''
    return $line.TrimEnd()
}

function Add-Paragraph($selection, $text, $style = 'Normal', $fontSize = $null, $bold = $false, $italic = $false, $align = $null, $spaceAfter = 6) {
    if ([string]::IsNullOrWhiteSpace($text)) {
        $selection.TypeParagraph() | Out-Null
        return
    }

    $selection.Style = $style
    if ($fontSize) { $selection.Font.Size = [single]$fontSize }
    if ($text -match '[\u3400-\u9FFF]') {
        $selection.Font.NameFarEast = 'Microsoft YaHei'
    }
    else {
        $selection.Font.Name = 'Calibri'
    }
    $selection.Range.NoProofing = 1
    $selection.Font.Bold = [int]$bold
    $selection.Font.Italic = [int]$italic
    if ($null -ne $align) { $selection.ParagraphFormat.Alignment = $align }
    $selection.ParagraphFormat.SpaceAfter = $spaceAfter
    $selection.TypeText($text)
    $selection.TypeParagraph() | Out-Null
    $selection.Font.Bold = 0
    $selection.Font.Italic = 0
}

function Add-ImageGrid($doc, $selection, $imageItems, $columns = 3, $maxWidthPoints = 150) {
    $rows = [Math]::Ceiling($imageItems.Count / $columns)
    $tableRange = $doc.Range($selection.Start, $selection.Start)
    $table = $doc.Tables.Add($tableRange, $rows, $columns)
    $table.Borders.Enable = 0
    $table.Rows.Alignment = 1
    $table.AllowAutoFit = $true

    for ($index = 0; $index -lt ($rows * $columns); $index++) {
        $row = [Math]::Floor($index / $columns) + 1
        $col = ($index % $columns) + 1
        $cell = $table.Cell($row, $col)
        $cell.Range.ParagraphFormat.Alignment = 1
        $cell.VerticalAlignment = 1

        if ($index -lt $imageItems.Count) {
            $selection.SetRange($cell.Range.Start, $cell.Range.Start)
            $shape = $selection.InlineShapes.AddPicture($imageItems[$index].Path)
            $shape.LockAspectRatio = -1
            if ($shape.Width -gt $maxWidthPoints) {
                $shape.Width = $maxWidthPoints
            }
            $selection.TypeParagraph() | Out-Null
            Add-Paragraph $selection $imageItems[$index].Label 'No Spacing' 9 $false $false 1 2
        }
    }

    $selection.SetRange($table.Range.End, $table.Range.End)
    $selection.TypeParagraph() | Out-Null
}

function Add-ImageStack($selection, $imageItems, $maxWidthPoints = 420) {
    foreach ($imageItem in $imageItems) {
        Add-SingleImage $selection $imageItem.Path $maxWidthPoints
        Add-Paragraph $selection $imageItem.Label 'No Spacing' 9.5 $false $false 1 6
    }
}

function Parse-MarkdownRow($line) {
    $trimmed = $line.Trim()
    if ($trimmed.StartsWith('|')) { $trimmed = $trimmed.Substring(1) }
    if ($trimmed.EndsWith('|')) { $trimmed = $trimmed.Substring(0, $trimmed.Length - 1) }
    return @($trimmed -split '\s*\|\s*' | ForEach-Object { Clean-Line($_.Trim()) })
}

function Add-MarkdownTable($doc, $selection, $tableLines) {
    $rows = @()
    foreach ($tableLine in $tableLines) {
        if ($tableLine -match '^\|\s*[-: ]+\|') { continue }
        $rows += ,(Parse-MarkdownRow $tableLine)
    }

    if ($rows.Count -eq 0) { return }

    $tableRange = $doc.Range($selection.Start, $selection.Start)
    $columnCount = $rows[0].Count
    $table = $doc.Tables.Add($tableRange, $rows.Count, $columnCount)
    $table.Style = 'Table Grid'
    $table.Rows.Alignment = 1
    $table.AllowAutoFit = $true

    for ($r = 1; $r -le $rows.Count; $r++) {
        for ($c = 1; $c -le $columnCount; $c++) {
            $cellRange = $table.Cell($r, $c).Range
            $cellRange.Text = $rows[$r - 1][$c - 1]
            $cellRange.Font.NameFarEast = 'Microsoft YaHei'
            $cellRange.Font.Name = 'Calibri'
            $cellRange.Font.Size = [single]10.5
            $cellRange.NoProofing = 1
            $cellRange.ParagraphFormat.SpaceAfter = 0
            if ($r -eq 1) {
                $cellRange.Font.Bold = 1
                $table.Cell($r, $c).Shading.BackgroundPatternColor = 15987699
            }
        }
    }

    $selection.SetRange($table.Range.End, $table.Range.End)
    $selection.TypeParagraph() | Out-Null
}

function Add-FlowDiagram($doc, $selection, $steps, $caption, $maxWidthPoints = 340) {
    for ($i = 0; $i -lt $steps.Count; $i++) {
        $stepRange = $doc.Range($selection.Start, $selection.Start)
        $stepTable = $doc.Tables.Add($stepRange, 1, 1)
        $stepTable.Rows.Alignment = 1
        $stepTable.AllowAutoFit = $false
        $stepTable.Columns.Item(1).Width = $maxWidthPoints
        $stepCell = $stepTable.Cell(1, 1)
        $stepCell.Range.Text = $steps[$i]
        $stepCell.Range.Font.NameFarEast = 'Microsoft YaHei'
        $stepCell.Range.Font.Name = 'Calibri'
        $stepCell.Range.Font.Size = [single]10.5
        $stepCell.Range.Font.Bold = 1
        $stepCell.Range.NoProofing = 1
        $stepCell.Range.ParagraphFormat.Alignment = 1
        $stepCell.VerticalAlignment = 1
        $stepCell.Shading.BackgroundPatternColor = 15132390
        $stepTable.Borders.Enable = 1
        $selection.SetRange($stepTable.Range.End, $stepTable.Range.End)
        $selection.TypeParagraph() | Out-Null

        if ($i -lt ($steps.Count - 1)) {
            Add-Paragraph $selection (From-CodePoints @(8595)) 'No Spacing' 16 $true $false 1 1
        }
    }

    Add-Paragraph $selection $caption 'Normal' 10 $false $true 1 8
}

function Add-CodeSnippet($doc, $selection, $title, $lines) {
    $tableRange = $doc.Range($selection.Start, $selection.Start)
    $table = $doc.Tables.Add($tableRange, $lines.Count + 1, 1)
    $table.Style = 'Table Grid'
    $table.Rows.Alignment = 1
    $table.AllowAutoFit = $true
    $table.Cell(1, 1).Range.Text = $title
    $table.Cell(1, 1).Range.Font.Bold = 1
    $table.Cell(1, 1).Range.NoProofing = 1
    $table.Cell(1, 1).Shading.BackgroundPatternColor = 15987699

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $cell = $table.Cell($i + 2, 1)
        $cell.Range.Text = $lines[$i]
        $cell.Range.Font.NameFarEast = 'Microsoft YaHei'
        $cell.Range.Font.Name = 'Consolas'
        $cell.Range.Font.Size = [single]9.5
        $cell.Range.NoProofing = 1
    }

    $selection.SetRange($table.Range.End, $table.Range.End)
    $selection.TypeParagraph() | Out-Null
}

function Add-FigureExplanation($selection, $englishText, $chineseText = '') {
    Add-Paragraph $selection ('Explanation: ' + $englishText) 'Normal' 10 $false $false 0 2
    if (-not [string]::IsNullOrWhiteSpace($chineseText)) {
        Add-Paragraph $selection ((From-CodePoints @(35828,26126,65306)) + $chineseText) 'Normal' 10 $false $false 0 8
    }
    else {
        Add-Paragraph $selection '' 'Normal'
    }
}

function Add-SingleImage($selection, $path, $maxWidthPoints = 360) {
    $shape = $selection.InlineShapes.AddPicture($path)
    $shape.LockAspectRatio = -1
    if ($shape.Width -gt $maxWidthPoints) {
        $shape.Width = $maxWidthPoints
    }
    $selection.TypeParagraph() | Out-Null
}

$lines = Get-Content -LiteralPath $src -Encoding UTF8

$cnCoverTitle = From-CodePoints @(31995,32479,20132,20184,19982,25216,26415,25991,26723)
$cnSystemName = From-CodePoints @(28151,21512,29616,23454,27827,27969,27700,36136,21487,35270,21270,31995,32479)
$cnToc = From-CodePoints @(30446,24405)
$cnLabel = From-CodePoints @(20013,25991)
$screenshotDir = 'C:\Users\ZHANGXIAOYA\Pictures\Screenshots\'
$hardwareDir = 'C:\Users\ZHANGXIAOYA\Documents\xwechat_files\wxid_u9bi6cwvf6xo22_53ec\temp\RWTemp\2026-04\9e20f478899dc29eb19741386f9343c8\'
$coverTitleEn = Clean-Line($lines[0].Substring(2))
$coverTitleCn = Clean-Line($lines[1].Substring(2))
$coverSystemEn = Clean-Line($lines[3].Substring(3))
$coverSystemCn = Clean-Line($lines[4].Substring(3))
$figure5Images = @(
    @{ Path = $hardwareDir + '8e5dada02cdf0a6ca54a83a859e4a4cd.jpg'; Label = 'Control box overview' },
    @{ Path = $hardwareDir + 'abab3affd9799194312e55cec4cb1058.jpg'; Label = 'Raspberry Pi and breadboard wiring' }
)
$figure7Images = @(
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 025740.png'; Label = 'Package Manager assets' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 025920.png'; Label = 'XR Plug-in Management' }
)
$figure8Images = @(
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 030835.png'; Label = 'WaterSystemManager references' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 030931.png'; Label = 'WaterSystemManager data and chart bindings' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 031235.png'; Label = 'mapswitch and map layer references' }
)
$figure9Images = @(
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 030801.png'; Label = 'PiSystemBridge inspector - upper section' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 030817.png'; Label = 'PiSystemBridge inspector - lower section' }
)
$figure10Images = @(
    @{ Path = 'C:\Users\ZHANGXIAOYA\Documents\xwechat_files\wxid_u9bi6cwvf6xo22_53ec\temp\RWTemp\2026-04\b69018776db9691c36ecc77e429f197d.png'; Label = 'ARP lookup and SSH login' }
)
$figure6Images = @(
    @{ Path = $hardwareDir + 'dca3cdad8c07293521244e017facb5de.jpg'; Label = 'Slider module detail' },
    @{ Path = $hardwareDir + '3ab8c581b19ae0823dfadbf8d9ec0984.jpg'; Label = 'Encoder wiring detail' },
    @{ Path = $hardwareDir + 'f8afc1e0104c71c616e0a3599c079b9b.jpg'; Label = 'Joystick module detail' }
)
$figure11Images = @(
    @{ Path = $screenshotDir + (From-CodePoints @(24038,19978,26059,38062)) + '.png'; Label = 'Encoder 1 / ' + (From-CodePoints @(24038,19978,26059,38062)) },
    @{ Path = $screenshotDir + (From-CodePoints @(21491,19978,26059,38062)) + '.png'; Label = 'Encoder 2 / ' + (From-CodePoints @(21491,19978,26059,38062)) },
    @{ Path = $screenshotDir + (From-CodePoints @(24038,19979,26059,38062)) + '.png'; Label = 'Encoder 3 / ' + (From-CodePoints @(24038,19979,26059,38062)) },
    @{ Path = $screenshotDir + (From-CodePoints @(24038,27178,26438)) + '.png'; Label = 'Slider 1 / ' + (From-CodePoints @(24038,27178,26438)) },
    @{ Path = $screenshotDir + (From-CodePoints @(19979,27178,26438)) + '.png'; Label = 'Slider 2 / ' + (From-CodePoints @(19979,27178,26438)) },
    @{ Path = $screenshotDir + (From-CodePoints @(25671,26438)) + '.png'; Label = 'Joystick / ' + (From-CodePoints @(25671,26438)) }
)
$mrRuntimeImage = $screenshotDir + 'Screenshot 2026-04-18 213449.png'
$figure3Images = @(
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 015501.png'; Label = 'Network configuration and send()' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 015525.png'; Label = 'Button callback registration' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 015546.png'; Label = 'encoder_loop() implementation' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 015633.png'; Label = 'read_adc() and ADC thresholds' },
    @{ Path = $screenshotDir + 'Screenshot 2026-04-20 015704.png'; Label = 'joystick_loop() and slider_loop()' }
)
$figure11Caption = 'Figure 11. Raspberry Pi terminal output for encoders, sliders, and joystick / ' +
    (From-CodePoints @(22270,49,49,46,26641,33683,27966,32456,30721,20986,26059,38062,28369,26438,21450,25671,26438,30340,32456,31471,36755,20986))
$figure1Caption = 'Figure 1. Overall system architecture / ' + (From-CodePoints @(22270,49,46,31995,32479,24635,35272,22270))
$figure2Caption = 'Figure 2. Data flow from Raspberry Pi to Unity / ' + (From-CodePoints @(22270,50,46,26641,33683,27966,21040,32,85,110,105,116,121,32,30340,25968,25454,27969,22270))
$figure3Caption = 'Figure 3. Key sections of pi_input_sender.py / ' + (From-CodePoints @(22270,51,46,112,105,95,105,110,112,117,116,95,115,101,110,100,101,114,46,112,121,32,20851,38190,20195,30721,25130,22270))
$figure4Caption = 'Figure 4. MCP3008 analog conversion flow / ' + (From-CodePoints @(22270,52,46,77,67,80,51,48,48,56,32,27169,25311,37327,36716,25442,27969,31243,22270))
$figure5Caption = 'Figure 5. Raspberry Pi and breadboard overview / ' +
    (From-CodePoints @(22270,53,46,26641,33683,27966,19982,38754,21253,26495,25972,20307,22270))
$figure6Caption = 'Figure 6. Joystick, slider, and encoder mapping / ' +
    (From-CodePoints @(22270,54,46,25671,26438,12289,28369,26438,19982,26059,38062,26144,23556,22270))
$figure7Caption = 'Figure 7. Unity project and XR package setup / ' +
    (From-CodePoints @(22270,55,46,85,110,105,116,121,32,39033,30446,19982,32,88,82,32,21253,37197,32622))
$figure8Caption = 'Figure 8. Key Unity scripts and scene relationship / ' +
    (From-CodePoints @(22270,56,46,85,110,105,116,121,32,20851,38190,33050,26412,19982,22330,26223,20851,31995,22270))
$figure9Caption = 'Figure 9. PiSystemBridge Inspector screenshot / ' +
    (From-CodePoints @(22270,57,46,80,105,83,121,115,116,101,109,66,114,105,100,103,101,32,73,110,115,112,101,99,116,111,114,32,25130,22270))
$figure10Caption = 'Figure 10. Raspberry Pi setup and SSH access / ' +
    (From-CodePoints @(22270,49,48,46,26641,33683,27966,37197,32622,19982,32,83,83,72,32,35775,38382,25130,22270))
$figure12Caption = 'Figure 12. MR runtime interface and data interaction / ' +
    (From-CodePoints @(22270,49,50,46,28151,21512,29616,23454,36816,34892,30028,38754,19982,25968,25454,20132,20184))
$figure1Steps = @(
    'Physical controllers',
    'Raspberry Pi Zero',
    'MCP3008 for analog inputs',
    'pi_input_sender.py',
    'UDP transport',
    'Unity MR application',
    'Map and water-quality interaction'
)
$figure2Steps = @(
    'Joystick, sliders, encoders',
    'GPIO or MCP3008 channel read',
    'Python input loop classifies state',
    'UDP message such as ENC / JOY / SLIDER / BTN',
    'PiSystemBridge receives on port 5005',
    'Unity updates map, river, RI, and data view'
)
$figure4Steps = @(
    'Analog movement on slider or joystick',
    'MCP3008 samples CH1 / CH3 / CH5 / CH6',
    'SPI sends digital values to Raspberry Pi',
    'spidev reads channel values in Python',
    'Threshold logic converts values into events',
    'UDP messages are sent to Unity'
)

$word = $null
$doc = $null
$insertedFigure12 = $false

try {
    $word = New-Object -ComObject Word.Application
    $word.Visible = $false
    $word.DisplayAlerts = 0
    $doc = $word.Documents.Add()
    $selection = $word.Selection

    $wdAlignParagraphCenter = 1
    $wdAlignParagraphLeft = 0
    $wdPageBreak = 7
    $wdHeaderFooterPrimary = 1
    $wdPageNumberAlignmentCenter = 1

    foreach ($styleName in @('Normal', 'Title', 'Subtitle', 'Heading 1', 'Heading 2', 'Heading 3', 'No Spacing')) {
        $style = $doc.Styles.Item($styleName)
        $style.Font.NameFarEast = 'Microsoft YaHei'
        $style.Font.Name = 'Calibri'
    }

    $doc.Styles.Item('Normal').Font.Size = [single]11
    $doc.Styles.Item('Normal').ParagraphFormat.SpaceAfter = 6
    $doc.Styles.Item('Normal').ParagraphFormat.LineSpacingRule = 0
    $doc.Styles.Item('Normal').ParagraphFormat.LineSpacing = 15

    $doc.Styles.Item('Heading 1').Font.Size = [single]15
    $doc.Styles.Item('Heading 1').Font.Bold = 1
    $doc.Styles.Item('Heading 1').ParagraphFormat.SpaceBefore = 12
    $doc.Styles.Item('Heading 1').ParagraphFormat.SpaceAfter = 6

    $doc.Styles.Item('Heading 2').Font.Size = [single]13
    $doc.Styles.Item('Heading 2').Font.Bold = 1
    $doc.Styles.Item('Heading 2').ParagraphFormat.SpaceBefore = 8
    $doc.Styles.Item('Heading 2').ParagraphFormat.SpaceAfter = 4

    $doc.Styles.Item('Heading 3').Font.Size = [single]11.5
    $doc.Styles.Item('Heading 3').Font.Bold = 1
    $doc.Styles.Item('Heading 3').ParagraphFormat.SpaceBefore = 6
    $doc.Styles.Item('Heading 3').ParagraphFormat.SpaceAfter = 3

    Add-Paragraph $selection $coverTitleEn 'Title' 18 $true $false $wdAlignParagraphCenter 8
    Add-Paragraph $selection $coverTitleCn 'Title' 18 $true $false $wdAlignParagraphCenter 14
    Add-Paragraph $selection $coverSystemEn 'Subtitle' 13 $false $false $wdAlignParagraphCenter 4
    Add-Paragraph $selection $coverSystemCn 'Subtitle' 13 $false $false $wdAlignParagraphCenter 18

    Add-Paragraph $selection 'Prepared by: Zhang Xiaoya' 'Normal' 11 $false $false $wdAlignParagraphCenter 4
    Add-Paragraph $selection 'Supervisor: Dr. Lam Meng Chun' 'Normal' 11 $false $false $wdAlignParagraphCenter 4
    Add-Paragraph $selection 'Organization: FTSM / MYXLab, UKM' 'Normal' 11 $false $false $wdAlignParagraphCenter 4
    Add-Paragraph $selection 'Date: 17 April 2026' 'Normal' 11 $false $false $wdAlignParagraphCenter 4

    $selection.InsertBreak($wdPageBreak)

    Add-Paragraph $selection ('Table of Contents / ' + $cnToc) 'Heading 1' 15 $true $false $wdAlignParagraphLeft 6
    $tocRange = $doc.Range($selection.Start, $selection.Start)
    $doc.TablesOfContents.Add($tocRange, $true, 1, 3, $true, $null, $true, $true, $true) | Out-Null

    $selection.SetRange($doc.Content.End - 1, $doc.Content.End - 1)
    $selection.InsertBreak($wdPageBreak)

    for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
        $line = [string]$lines[$lineIndex]

        if (-not $insertedFigure12 -and $line -match '^## 6\. ') {
            Add-SingleImage $selection $mrRuntimeImage 360
            Add-Paragraph $selection $figure12Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This screenshot shows the MR runtime interface after Unity has received controller input, including river selection, map style controls, data panels, and overlaid water-quality views.'
            $insertedFigure12 = $true
        }

        if ($line -match '^```') {
            while (($lineIndex + 1) -lt $lines.Count -and ([string]$lines[$lineIndex + 1]) -notmatch '^```') {
                $lineIndex++
            }
            if (($lineIndex + 1) -lt $lines.Count) {
                $lineIndex++
            }
            continue
        }

        if ($line -match '^# ') {
            Add-Paragraph $selection (Clean-Line($line.Substring(2))) 'Title' 16 $true $false $wdAlignParagraphCenter 10
            continue
        }
        if ($line -match '^## ') {
            Add-Paragraph $selection (Clean-Line($line.Substring(3))) 'Heading 1' 15 $true $false $wdAlignParagraphLeft 6
            continue
        }
        if ($line -match '^### ') {
            Add-Paragraph $selection (Clean-Line($line.Substring(4))) 'Heading 2' 13 $true $false $wdAlignParagraphLeft 4
            continue
        }
        if ($line -match '^#### ') {
            Add-Paragraph $selection (Clean-Line($line.Substring(5))) 'Heading 3' 11.5 $true $false $wdAlignParagraphLeft 3
            continue
        }
        if ((Clean-Line($line)) -eq 'English') {
            Add-Paragraph $selection 'English' 'Normal' 11 $true $true $wdAlignParagraphLeft 2
            continue
        }
        if ((Clean-Line($line)) -eq $cnLabel) {
            Add-Paragraph $selection $cnLabel 'Normal' 11 $true $true $wdAlignParagraphLeft 2
            continue
        }
        if ($line -match '^\- ') {
            Add-Paragraph $selection (([char]8226) + ' ' + (Clean-Line($line.Substring(2)))) 'Normal' 11 $false $false $wdAlignParagraphLeft 2
            continue
        }
        if ($line -match '^\d+\. ') {
            Add-Paragraph $selection (Clean-Line($line)) 'Normal' 11 $false $false $wdAlignParagraphLeft 2
            continue
        }
        if ($line -match '^\|') {
            $tableLines = @()
            while ($lineIndex -lt $lines.Count -and ([string]$lines[$lineIndex]) -match '^\|') {
                $tableLines += [string]$lines[$lineIndex]
                $lineIndex++
            }
            $lineIndex--
            Add-MarkdownTable $doc $selection $tableLines
            continue
        }
        if ($line -match '^---+$') {
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 1 here:')) {
            Add-FlowDiagram $doc $selection $figure1Steps $figure1Caption 320
            Add-FigureExplanation -selection $selection -englishText 'This figure summarizes the full delivery path from physical input hardware to Raspberry Pi processing, UDP transport, and Unity-side visual interaction.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 2 here:')) {
            Add-FlowDiagram $doc $selection $figure2Steps $figure2Caption 320
            Add-FigureExplanation -selection $selection -englishText 'This figure focuses on runtime data movement, showing how controller state is sampled, translated into protocol messages, and consumed by PiSystemBridge in Unity.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 3 here:')) {
            Add-ImageStack $selection $figure3Images 420
            Add-Paragraph $selection $figure3Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'These screenshots show the actual Raspberry Pi runtime script sections used in the project, including network configuration, button callbacks, encoder handling, ADC reading, and the joystick and slider loops.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 4 here:')) {
            Add-FlowDiagram $doc $selection $figure4Steps $figure4Caption 320
            Add-FigureExplanation -selection $selection -englishText 'This figure explains how MCP3008 converts analog voltage changes from the joystick and sliders into digital channel values that Python can read and forward to Unity.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 5 here:')) {
            Add-ImageGrid $doc $selection $figure5Images 2 220
            Add-Paragraph $selection $figure5Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure documents the physical prototype, including the control box layout and the Raspberry Pi with breadboard wiring used during integration.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 6 here:')) {
            Add-ImageGrid $doc $selection $figure6Images 3 145
            Add-Paragraph $selection $figure6Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure highlights the mapping details of the slider, encoder, and joystick modules so that each physical control can be matched to its corresponding GPIO or ADC channel.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 7 here:')) {
            Add-ImageStack $selection $figure7Images 430
            Add-Paragraph $selection $figure7Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure records the Unity-side environment setup, combining package acquisition evidence with XR Plug-in Management settings that enable OpenXR and Meta Quest feature groups for the project.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 8 here:')) {
            Add-ImageStack $selection $figure8Images 430
            Add-Paragraph $selection $figure8Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure shows the main Unity-side logic relationships by highlighting WaterSystemManager bindings and map switching references used after Raspberry Pi input is received.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 9 here:')) {
            Add-ImageStack $selection $figure9Images 430
            Add-Paragraph $selection $figure9Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure captures the PiSystemBridge Inspector configuration, including UDP port settings, scene references, slider ranges, menu panels, river selection parameters, and runtime state values.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 10 here:')) {
            Add-ImageStack $selection $figure10Images 430
            Add-Paragraph $selection $figure10Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure shows the Raspberry Pi access workflow from Windows PowerShell, including address lookup and successful SSH login to the target device before script execution.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 11 here:')) {
            Add-ImageGrid $doc $selection $figure11Images 3 150
            Add-Paragraph $selection $figure11Caption 'Normal' 10 $false $true $wdAlignParagraphCenter 8
            Add-FigureExplanation -selection $selection -englishText 'This figure provides terminal-level validation evidence that encoder rotation, joystick movement, slider changes, and button presses were all detected and emitted as runtime messages.'
            continue
        }
        if ((Clean-Line($line)).StartsWith('[Insert Figure 12 here:')) {
            continue
        }
        if ($line -match '^\s*$') {
            Add-Paragraph $selection '' 'Normal'
            continue
        }

        Add-Paragraph $selection (Clean-Line($line)) 'Normal' 11 $false $false $wdAlignParagraphLeft 4
    }

    foreach ($section in $doc.Sections) {
        $footer = $section.Footers.Item($wdHeaderFooterPrimary)
        $footer.PageNumbers.RestartNumberingAtSection = $false
        $footer.PageNumbers.Add($wdPageNumberAlignmentCenter, $true) | Out-Null
    }

    $doc.Repaginate()
    if ($doc.TablesOfContents.Count -gt 0) {
        $doc.TablesOfContents.Item(1).Update() | Out-Null
    }

    $doc.SaveAs2([ref]$outProject, [ref]16)
    Copy-Item -LiteralPath $outProject -Destination $outDownloads -Force
    Write-Output "Created: $outProject`nCreated: $outDownloads"
}
finally {
    if ($doc -ne $null) {
        try { $doc.Close(0) | Out-Null } catch {}
        try { [System.Runtime.InteropServices.Marshal]::ReleaseComObject($doc) | Out-Null } catch {}
    }
    if ($word -ne $null) {
        try { $word.Quit() | Out-Null } catch {}
        try { [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null } catch {}
    }
}
