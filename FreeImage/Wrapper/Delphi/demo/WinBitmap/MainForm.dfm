object fwbMainForm: TfwbMainForm
  Left = 205
  Top = 206
  Width = 696
  Height = 480
  Caption = 'FreeWinBitmap - MainDemo'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Shell Dlg 2'
  Font.Style = []
  Menu = MainMenu
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  OnPaint = FormPaint
  OnResize = FormResize
  PixelsPerInch = 96
  TextHeight = 13
  object StatusBar: TStatusBar
    Left = 0
    Top = 411
    Width = 688
    Height = 23
    Panels = <
      item
        Alignment = taCenter
        Width = 120
      end
      item
        Alignment = taCenter
        Width = 80
      end
      item
        Width = 50
      end>
  end
  object MainMenu: TMainMenu
    Left = 120
    Top = 48
    object mnuFile: TMenuItem
      Caption = '&File'
      object mnuFileOpen: TMenuItem
        Caption = '&Open'
        OnClick = mnuFileOpenClick
      end
      object mnuExit: TMenuItem
        Caption = 'E&xit'
        OnClick = mnuExitClick
      end
    end
    object mnuImage: TMenuItem
      Caption = 'Image'
      object mnuImageFlip: TMenuItem
        Caption = 'Flip'
        object mnuFlipHorz: TMenuItem
          Caption = 'Horizontal'
          OnClick = mnuFlipHorzClick
        end
        object mnuFlipVert: TMenuItem
          Caption = 'Vertical'
          OnClick = mnuFlipHorzClick
        end
      end
      object mnuConvert: TMenuItem
        Caption = 'Convert'
        object mnuTo4Bits: TMenuItem
          Caption = 'To 4 Bits'
          OnClick = mnuFlipHorzClick
        end
        object mnuTo8Bits: TMenuItem
          Caption = 'To 8 Bits'
          OnClick = mnuFlipHorzClick
        end
        object mnuTo16Bits555: TMenuItem
          Caption = 'To 16 Bits (555)'
          OnClick = mnuFlipHorzClick
        end
        object mnuTo16Bits565: TMenuItem
          Caption = 'To 16 Bits (565)'
          OnClick = mnuFlipHorzClick
        end
        object mnuTo24Bits: TMenuItem
          Caption = 'To 24 Bits'
          OnClick = mnuFlipHorzClick
        end
        object mnuTo32Bits: TMenuItem
          Caption = 'To 32 Bits'
          OnClick = mnuFlipHorzClick
        end
        object mnuDither: TMenuItem
          Caption = 'Dither'
          OnClick = mnuFlipHorzClick
        end
        object mnuQuantize: TMenuItem
          Caption = 'Quantize'
          OnClick = mnuFlipHorzClick
        end
        object mnuGrayScale: TMenuItem
          Caption = 'GrayScale'
          OnClick = mnuFlipHorzClick
        end
      end
      object mnuRotate: TMenuItem
        Caption = 'Rotate'
        object mnuClockwise: TMenuItem
          Caption = 'Clockwise'
          OnClick = mnuFlipHorzClick
        end
        object mnuAntiClockwise: TMenuItem
          Caption = 'AntiClockwise'
          OnClick = mnuFlipHorzClick
        end
      end
      object mnuInvert: TMenuItem
        Caption = 'Invert'
        OnClick = mnuFlipHorzClick
      end
      object mnuClear: TMenuItem
        Caption = 'Clear'
        OnClick = mnuFlipHorzClick
      end
    end
  end
  object OD: TOpenDialog
    Title = 'Open file ...'
    Left = 152
    Top = 48
  end
end
