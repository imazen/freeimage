unit FreeBitmap;

// ==========================================================
//
// Delphi wrapper for FreeImage 3
//
// Design and implementation by
// - Anatoliy Pulyaevskiy (xvel84@rambler.ru)
//
// Contributors:
// - Enzo Costantini (enzocostantini@libero.it)
//
// This file is part of FreeImage 3
//
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS, WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, WITHOUT LIMITATION, WARRANTIES
// THAT THE COVERED CODE IS FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE
// OR NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE COVERED
// CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE DEFECTIVE IN ANY RESPECT, YOU (NOT
// THE INITIAL DEVELOPER OR ANY OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY
// SERVICING, REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN ESSENTIAL
// PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS AUTHORIZED HEREUNDER EXCEPT UNDER
// THIS DISCLAIMER.
//
// Use at your own risk!
//
// ==========================================================
//
// From begining all code of this file is based on C++ wrapper to
// FreeImage - FreeImagePlus. So it is simply translation from C on Delphi Pascal
// with just some minor changes.
//
// ==========================================================

interface

uses
  SysUtils, Classes, Windows, FreeImage;

type
  TFreeStretchFilter = (
    sfBox,
    sfBicubic,
    sfBilinear,
    sfBSpline,
    sfCatmullRom,
    sfLanczos3
  );

  { TFreeObject }

  TFreeObject = class
  public
    function IsValid: Boolean; virtual;
  end;

// forward declarations
  TFreeMemoryIO = class;

  { TFreeBitmap }

  TFreeBitmap = class(TFreeObject)
  private
    FDib: PFIBITMAP;
    FOnChange: TNotifyEvent;
  protected
    function Replace(NewDib: PFIBITMAP): Boolean;
  public
    // construction & destruction
    constructor Create(ImageType: FREE_IMAGE_TYPE = FIT_BITMAP; Width: Integer = 0; Height: Integer = 0; Bpp: Integer = 0);
    destructor Destroy; override;
    function SetSize(ImageType: FREE_IMAGE_TYPE; Width, Height, Bpp: Integer; RedMask: Cardinal = 0; GreenMask: Cardinal = 0; BlueMask: Cardinal = 0): Boolean;
    // change notification
    procedure Change; dynamic;
    // copying
    procedure Assign(Source: TFreeBitmap); overload;
    procedure Assign(Source: PFIBITMAP); overload;
    function CopySubImage(Left, Top, Right, Bottom: Integer; Dest: TFreeBitmap): Boolean;
    function PasteSubImage(Src: TFreeBitmap; Left, Top: Integer; Alpha: Integer = 256): Boolean;    
    // clearing
    procedure Clear;
    // load functions
    function Load(const FileName: string; Flag: Integer = 0): Boolean;
    function LoadFromHandle(IO: PFreeImageIO; Handle: fi_handle; Flag: Integer = 0): Boolean;
    function LoadFromMemory(MemIO: TFreeMemoryIO; Flag: Integer = 0): Boolean;
    // save functions
    function Save(const FileName: string; Flag: Integer = 0): Boolean;
    function SaveToHandle(fif: FREE_IMAGE_FORMAT; IO: PFreeImageIO; Handle: fi_handle; Flag: Integer = 0): Boolean;
    function SaveToMemory(fif: FREE_IMAGE_FORMAT; MemIO: TFreeMemoryIO; Flag: Integer = 0): Boolean;
    // image information
    function GetImageType: FREE_IMAGE_TYPE;
    function GetWidth: Integer;
    function GetHeight: Integer;
    function GetScanWidth: Integer;
    function IsValid: Boolean; override;
    function GetInfo: PBitmapInfo;
    function GetInfoHeader: PBitmapInfoHeader;
    function GetImageSize: Cardinal;
    function GetBitsPerPixel: Integer;
    function GetLine: Integer;
    function GetHorizontalResolution: Integer;
    function GetVerticalResolution: Integer;
    procedure SetHorizontalResolution(Value: Integer);
    procedure SetVerticalResolution(Value: Integer);
    // palette operations
    function GetPalette: PRGBQUAD;
    function GetPaletteSize: Integer;
    function GetColorsUsed: Integer;
    function GetColorType: FREE_IMAGE_COLOR_TYPE;
    function IsGrayScale: Boolean;
    // pixels access
    function AccessPixels: PByte;
    function GetScanLine(ScanLine: Integer): PByte;
    function GetPixelIndex(X, Y: Cardinal; var Value: PByte): Boolean;
    function GetPixelColor(X, Y: Cardinal; var Value: PRGBQUAD): Boolean;
    function SetPixelIndex(X, Y: Cardinal; Value: PByte): Boolean;
    function SetPixelColor(X, Y: Cardinal; Value: PRGBQUAD): Boolean;
    // convertion
    function ConvertToStandartType(ScaleLinear: Boolean): Boolean;
    function ConvertToType(ImageType: FREE_IMAGE_TYPE; ScaleLinear: Boolean): Boolean;
    function Threshold(T: Byte): Boolean;
    function ConvertTo4Bits: Boolean;
    function ConvertTo8Bits: Boolean;
    function ConvertTo16Bits555: Boolean;
    function ConvertTo16Bits565: Boolean;
    function ConvertTo24Bits: Boolean;
    function ConvertTo32Bits: Boolean;
    function ConvertToGrayscale: Boolean;
    function ColorQuantize(Algorithm: FREE_IMAGE_QUANTIZE): Boolean;
    function Dither(Algorithm: FREE_IMAGE_DITHER): Boolean;
    // transparency
    function IsTransparent: Boolean;
    function GetTransparencyCount: Cardinal;
    function GetTransparencyTable: PByte;
    procedure SetTransparencyTable(Table: PByte; Count: Integer);
    function HasFileBkColor: Boolean;
    function GetFileBkColor(var BkColor: PRGBQuad): Boolean;
    function SetFileBkColor(BkColor: PRGBQuad): Boolean;
    // channel processing routines
    function GetChannel(Bitmap: TFreeBitmap; Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
    function SetChannel(Bitmap: TFreeBitmap; Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
    function SplitChannels(RedChannel, GreenChannel, BlueChannel: TFreeBitmap): Boolean;
    function CombineChannels(Red, Green, Blue: TFreeBitmap): Boolean;
    // rotation and flipping
    function RotateEx(Angle, XShift, YShift, XOrigin, YOrigin: Double; UseMask: Boolean): Boolean;
    function Rotate(Angle: Double): Boolean;
    function FlipHorizontal: Boolean;
    function FlipVertical: Boolean;
    // color manipulation routines
    function Invert: Boolean;
    function AdjustCurve(Lut: PByte; Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
    function AdjustGamma(Gamma: Double): Boolean;
    function AdjustBrightness(Percentage: Double): Boolean;
    function AdjustContrast(Percentage: Double): Boolean;
    function GetHistogram(Histo: PDWORD; Channel: FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK): Boolean;
    // upsampling / downsampling
    procedure MakeThumbnail(const Width, Height: Integer; DestBitmap: TFreeBitmap);
    function Rescale(NewWidth, NewHeight: Integer; Filter: TFreeStretchFilter; Dest: TFreeBitmap = nil): Boolean;
    { Properties }
    property Dib: PFIBITMAP read FDib;
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
  end;
  
  { TFreeWinBitmap }

  TFreeWinBitmap = class(TFreeBitmap)
  private
    FDeleteMe: Boolean;     // True - need to delete FDisplayDib
    FDisplayDib: PFIBITMAP; // Image that paints on DC
  public
    constructor Create(ImageType: FREE_IMAGE_TYPE = FIT_BITMAP; Width: Integer = 0; Height: Integer = 0; Bpp: Integer = 0);
    destructor Destroy; override;

    function CopyToHandle: THandle;
    function CopyFromHandle(HMem: THandle): Boolean;
    function CopyFromBitmap(HBmp: HBITMAP): Boolean;
    function CopyToBitmapH: HBITMAP;
    function CopyToClipBoard(NewOwner: HWND): Boolean;
    function PasteFromClipBoard: Boolean;
    function CaptureWindow(ApplicationWindow, SelectedWindow: HWND): Boolean;

    procedure Draw(DC: HDC; Rect: TRect);
    procedure DrawEx(DC: HDC; Rect: TRect; UseFileBkg: Boolean = False; AppBkColor: PRGBQuad = nil; Bg: PFIBITMAP = nil);
  end;

  { TFreeMemoryIO }

  TFreeMemoryIO = class(TFreeObject)
  private
    FHMem: PFIMEMORY;
  public
    // construction and destruction
    constructor Create(Data: PByte = nil; SizeInBytes: DWORD = 0);
    destructor Destroy; override;

    function GetFileType: FREE_IMAGE_FORMAT;
    function Read(fif: FREE_IMAGE_FORMAT; Flag: Integer = 0): PFIBITMAP;
    function Write(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP; Flag: Integer = 0): Boolean;
    function Tell: Longint;
    function Seek(Offset: Longint; Origin: Integer): Boolean;
    function Acquire(var Data: PByte; var SizeInBytes: DWORD): Boolean;
    // overriden methods
    function IsValid: Boolean; override;
  end;

  { TFreeMultiBitmap }
  
  TFreeMultiBitmap = class(TFreeObject)
  private
    FMPage: PFIMULTIBITMAP;
    FMemoryCache: Boolean;
  public
    // constructor and destructor
    constructor Create(KeepCacheInMemory: Boolean = False);
    destructor Destroy; override;

    function Open(const FileName: string; CreateNew, ReadOnly: Boolean): Boolean;
    function Close(Flags: Integer = 0): Boolean;
    function GetPageCount: Integer;
    procedure AppendPage(Bitmap: TFreeBitmap);
    procedure InsertPage(Page: Integer; Bitmap: TFreeBitmap);
    procedure DeletePage(Page: Integer);
    function MovePage(Target, Source: Integer): Boolean;
    procedure LockPage(Page: Integer; DestBitmap: TFreeBitmap);
    procedure UnlockPage(Bitmap: TFreeBitmap; Changed: Boolean);
    function GetLockedPageNumbers(var Pages: Integer; var Count: Integer): Boolean;    
    // overriden methods
    function IsValid: Boolean; override;

    // properties
    // change of this property influences only on the next opening of a file
    property MemoryCache: Boolean read FMemoryCache write FMemoryCache;
  end;

implementation

//uses ConvUtils;

{ TFreeObject }

function TFreeObject.IsValid: Boolean;
begin
  Result := False
end;

{ TFreeBitmap }

function TFreeBitmap.AccessPixels: PByte;
begin
  Result := FreeImage_GetBits(FDib)
end;

function TFreeBitmap.AdjustBrightness(Percentage: Double): Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_AdjustBrightness(FDib, Percentage);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.AdjustContrast(Percentage: Double): Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_AdjustContrast(FDib, Percentage);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.AdjustCurve(Lut: PByte;
  Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_AdjustCurve(FDib, Lut, Channel);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.AdjustGamma(Gamma: Double): Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_AdjustGamma(FDib, Gamma);
    Change;
  end
  else
    Result := False
end;

procedure TFreeBitmap.Assign(Source: TFreeBitmap);
begin
  if Source <> Self then
  begin
    if Source <> nil then
      Assign(Source.FDib)
    else
      Clear;
  end;
end;

procedure TFreeBitmap.Assign(Source: PFIBITMAP);
var
  Clone: PFIBITMAP;
begin
  if Source = nil then
    Clear
  else
  begin
    Clone := FreeImage_Clone(Source);
    Replace(Clone)
  end
end;

procedure TFreeBitmap.Change;
begin
  if Assigned(FOnChange) then FOnChange(Self)
end;

procedure TFreeBitmap.Clear;
begin
  if FDib <> nil then
  begin
    FreeImage_Unload(FDib);
    FDib := nil;
    Change;
  end;
end;

function TFreeBitmap.ColorQuantize(
  Algorithm: FREE_IMAGE_QUANTIZE): Boolean;
var
  dib8: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib8 := FreeImage_ColorQuantize(FDib, Algorithm);
    Result := Replace(dib8);
  end
  else
    Result := False;
end;

function TFreeBitmap.CombineChannels(Red, Green,
  Blue: TFreeBitmap): Boolean;
var
  Width, Height: Integer;
begin
  if FDib = nil then
  begin
    Width := Red.GetWidth;
    Height := Red.GetHeight;
    FDib := FreeImage_Allocate(Width, Height, 24, FI_RGBA_RED_MASK,
            FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
  end;

  if FDib <> nil then
  begin
    Result := FreeImage_SetChannel(FDib, Red.FDib, FICC_RED) and
              FreeImage_SetChannel(FDib, Green.FDib, FICC_GREEN) and
              FreeImage_SetChannel(FDib, Blue.FDib, FICC_BLUE);

    Change
  end
  else
    Result := False;
end;

function TFreeBitmap.ConvertTo16Bits555: Boolean;
var
  dib16_555: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib16_555 := FreeImage_ConvertTo16Bits555(FDib);
    Result := Replace(dib16_555);
  end
  else
    Result := False
end;

function TFreeBitmap.ConvertTo16Bits565: Boolean;
var
  dib16_565: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib16_565 := FreeImage_ConvertTo16Bits565(FDib);
    Result := Replace(dib16_565);
  end
  else
    Result := False
end;

function TFreeBitmap.ConvertTo24Bits: Boolean;
var
  dibRGB: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dibRGB := FreeImage_ConvertTo24Bits(FDib);
    Result := Replace(dibRGB);
  end
  else
    Result := False
end;

function TFreeBitmap.ConvertTo32Bits: Boolean;
var
  dib32: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib32 := FreeImage_ConvertTo32Bits(FDib);
    Result := Replace(dib32);
  end
  else
    Result := False
end;

function TFreeBitmap.ConvertTo4Bits: Boolean;
var
  dib4: PFIBITMAP;
begin
  Result := False;
  if IsValid then
  begin
    dib4 := FreeImage_ConvertTo4Bits(FDib);
    Result := Replace(dib4);
  end;
end;

function TFreeBitmap.ConvertTo8Bits: Boolean;
var
  dib8: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib8 := FreeImage_ConvertTo8Bits(FDib);
    Result := Replace(dib8);
  end
  else
    Result := False
end;

function TFreeBitmap.ConvertToGrayscale: Boolean;
var
  ColorType: FREE_IMAGE_COLOR_TYPE;
begin
  Result := False;

  if FDib <> nil then
  begin
    ColorType := FreeImage_GetColorType(FDib);

    if (ColorType in [FIC_PALETTE, FIC_MINISWHITE]) then
    begin
      // convert the palette to 24-bit, then to 8-bit
      Result := ConvertTo24Bits;
      Result := Result and ConvertTo8Bits;
    end
    else
    if FreeImage_GetBPP(FDib) <> 8 then
      // convert the bitmap to 8-bit grayscale
      Result := ConvertTo8Bits
  end
end;

function TFreeBitmap.ConvertToStandartType(ScaleLinear: Boolean): Boolean;
var
  dibStandart: PFIBITMAP;
begin
  if IsValid then
  begin
    dibStandart := FreeImage_ConvertToStandardType(FDib, ScaleLinear);
    Result := Replace(dibStandart);
  end
  else
    Result := False;
end;

function TFreeBitmap.ConvertToType(ImageType: FREE_IMAGE_TYPE;
  ScaleLinear: Boolean): Boolean;
var
  dib: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib := FreeImage_ConvertToType(FDib, ImageType, ScaleLinear);
    Result := Replace(dib)
  end
  else
    Result := False
end;

function TFreeBitmap.CopySubImage(Left, Top, Right, Bottom: Integer;
  Dest: TFreeBitmap): Boolean;
begin
  if FDib <> nil then
  begin
    Dest.FDib := FreeImage_Copy(FDib, Left, Top, Right, Bottom);
    Result := Dest.IsValid;
  end else
    Result := False;
end;

constructor TFreeBitmap.Create(ImageType: FREE_IMAGE_TYPE; Width, Height,
  Bpp: Integer);
begin
  FDib := nil;
  if (Width > 0) and (Height > 0) and (Bpp > 0) then
    SetSize(ImageType, Width, Height, Bpp);
end;

destructor TFreeBitmap.Destroy;
begin
  if FDib <> nil then
    FreeImage_Unload(FDib);
  inherited;
end;

function TFreeBitmap.Dither(Algorithm: FREE_IMAGE_DITHER): Boolean;
var
  dib: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib := FreeImage_Dither(FDib, Algorithm);
    Result := Replace(dib);
  end
  else
    Result := False;
end;

function TFreeBitmap.FlipHorizontal: Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_FlipHorizontal(FDib);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.FlipVertical: Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_FlipVertical(FDib);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.GetBitsPerPixel: Integer;
begin
  Result := FreeImage_GetBPP(FDib)
end;

function TFreeBitmap.GetChannel(Bitmap: TFreeBitmap;
  Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
begin
  if FDib <> nil then
  begin
    Bitmap.FDib := FreeImage_GetChannel(FDib, Channel);
    Result := Bitmap.IsValid;
  end
  else
    Result := False
end;

function TFreeBitmap.GetColorsUsed: Integer;
begin
  Result := FreeImage_GetColorsUsed(FDib)
end;

function TFreeBitmap.GetColorType: FREE_IMAGE_COLOR_TYPE;
begin
  Result := FreeImage_GetColorType(FDib)
end;

function TFreeBitmap.GetFileBkColor(var BkColor: PRGBQuad): Boolean;
begin
  Result := FreeImage_GetBackgroundColor(FDib, BkColor)
end;

function TFreeBitmap.GetHeight: Integer;
begin
  Result := FreeImage_GetHeight(FDib)
end;

function TFreeBitmap.GetHistogram(Histo: PDWORD;
  Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
begin
  if FDib <> nil then
    Result := FreeImage_GetHistogram(FDib, Histo, Channel)
  else
    Result := False
end;

function TFreeBitmap.GetHorizontalResolution: Integer;
begin
  Result := FreeImage_GetDotsPerMeterX(FDib) div 100
end;

function TFreeBitmap.GetImageSize: Cardinal;
begin
  Result := FreeImage_GetDIBSize(FDib);
end;

function TFreeBitmap.GetImageType: FREE_IMAGE_TYPE;
begin
  Result := FreeImage_GetImageType(FDib)
end;

function TFreeBitmap.GetInfo: PBitmapInfo;
begin
  Result := FreeImage_GetInfo(FDib^)
end;

function TFreeBitmap.GetInfoHeader: PBITMAPINFOHEADER;
begin
  Result := FreeImage_GetInfoHeader(FDib)
end;

function TFreeBitmap.GetLine: Integer;
begin
  Result := FreeImage_GetLine(FDib)
end;

function TFreeBitmap.GetPalette: PRGBQUAD;
begin
  Result := FreeImage_GetPalette(FDib)
end;

function TFreeBitmap.GetPaletteSize: Integer;
begin
  Result := FreeImage_GetColorsUsed(FDib) * SizeOf(RGBQUAD)
end;

function TFreeBitmap.GetPixelColor(X, Y: Cardinal;
  var Value: PRGBQUAD): Boolean;
begin
  Result := FreeImage_GetPixelColor(FDib, X, Y, Value)
end;

function TFreeBitmap.GetPixelIndex(X, Y: Cardinal;
  var Value: PByte): Boolean;
begin
  Result := FreeImage_GetPixelIndex(FDib, X, Y, Value)
end;

function TFreeBitmap.GetScanLine(ScanLine: Integer): PByte;
var
  H: Integer;
begin
  H := FreeImage_GetHeight(FDib);
  if ScanLine < H then
    Result := FreeImage_GetScanLine(FDib, ScanLine)
  else
    Result := nil;
end;

function TFreeBitmap.GetScanWidth: Integer;
begin
  Result := FreeImage_GetPitch(FDib)
end;

function TFreeBitmap.GetTransparencyCount: Cardinal;
begin
  Result := FreeImage_GetTransparencyCount(FDib)
end;

function TFreeBitmap.GetTransparencyTable: PByte;
begin
  Result := FreeImage_GetTransparencyTable(FDib)
end;

function TFreeBitmap.GetVerticalResolution: Integer;
begin
  Result := FreeImage_GetDotsPerMeterY(Fdib) div 100
end;

function TFreeBitmap.GetWidth: Integer;
begin
  Result := FreeImage_GetWidth(FDib)
end;

function TFreeBitmap.HasFileBkColor: Boolean;
begin
  Result := FreeImage_HasBackgroundColor(FDib)
end;

function TFreeBitmap.Invert: Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_Invert(FDib);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.IsGrayScale: Boolean;
begin
  Result := (FreeImage_GetBPP(FDib) = 8)
            and (FreeImage_GetColorType(FDib) = FIC_PALETTE); 
end;

function TFreeBitmap.IsTransparent: Boolean;
begin
  Result := FreeImage_IsTransparent(FDib);
end;

function TFreeBitmap.IsValid: Boolean;
begin
  Result := FDib <> nil
end;

function TFreeBitmap.Load(const FileName: string; Flag: Integer): Boolean;
var
  fif: FREE_IMAGE_FORMAT;
begin

  // check the file signature and get its format
  fif := FreeImage_GetFileType(PChar(Filename), 0);
  if fif = FIF_UNKNOWN then
    // no signature?
    // try to guess the file format from the file extention
    fif := FreeImage_GetFIFFromFilename(PChar(FileName));

    // check that the plugin has reading capabilities ...
    if (fif <> FIF_UNKNOWN) and FreeImage_FIFSupportsReading(FIF) then
    begin
      // free the previous dib
      if FDib <> nil then
        FreeImage_Unload(dib);

      // load the file
      FDib := FreeImage_Load(fif, PChar(FileName), Flag);

      Change;
      Result := IsValid;
    end else
      Result := False;
end;

function TFreeBitmap.LoadFromHandle(IO: PFreeImageIO; Handle: fi_handle;
  Flag: Integer): Boolean;
var
  fif: FREE_IMAGE_FORMAT;
begin
  // check the file signature and get its format
  fif := FreeImage_GetFileTypeFromHandle(IO, Handle, 16);
  if (fif <> FIF_UNKNOWN) and FreeImage_FIFSupportsReading(fif) then
  begin
    // free the previous dib
    if FDib <> nil then
      FreeImage_Unload(FDib);

    // load the file
    FDib := FreeImage_LoadFromHandle(fif, IO, Handle, Flag);

    Change;
    Result := IsValid;
  end else
    Result := False;
end;

function TFreeBitmap.LoadFromMemory(MemIO: TFreeMemoryIO;
  Flag: Integer): Boolean;
var
  fif: FREE_IMAGE_FORMAT;
begin

  // check the file signature and get its format
  fif := MemIO.GetFileType;
  if (fif <> FIF_UNKNOWN) and FreeImage_FIFSupportsReading(fif) then
  begin
    // free the previous dib
    if FDib <> nil then
      FreeImage_Unload(FDib);

    // load the file
    FDib := MemIO.Read(fif, Flag);

    Result := IsValid;
    Change;
  end else
    Result := False;
end;

const
  ThumbSize = 150;

procedure TFreeBitmap.MakeThumbnail(const Width, Height: Integer;
  DestBitmap: TFreeBitmap);
type
  PRGB24 = ^TRGB24;
  TRGB24 = packed record
    B: Byte;
    G: Byte;
    R: Byte;
  end;
var
  x, y, ix, iy: integer;
  x1, x2, x3: integer;

  xscale, yscale: single;
  iRed, iGrn, iBlu, iRatio: Longword;
  p, c1, c2, c3, c4, c5: TRGB24;
  pt, pt1: PRGB24;
  iSrc, iDst, s1: integer;
  i, j, r, g, b, tmpY: integer;

  RowDest, RowSource, RowSourceStart: integer;
  w, h: Integer;
  dxmin, dymin: integer;
  ny1, ny2, ny3: integer;
  dx, dy: integer;
  lutX, lutY: array of integer;

  SrcBmp, DestBmp: PFIBITMAP;
begin
  if not IsValid then Exit;

  if (GetWidth <= ThumbSize) and (GetHeight <= ThumbSize) then
  begin
    DestBitmap.Assign(Self);
    Exit;
  end;

  w := Width;
  h := Height;

  // prepare bitmaps
  if GetBitsPerPixel <> 24 then
    SrcBmp := FreeImage_ConvertTo24Bits(FDib)
  else
    SrcBmp := FDib;
  DestBmp := FreeImage_Allocate(w, h, 24);
  Assert(DestBmp <> nil, 'TFreeBitmap.MakeThumbnail error');

{  iDst := (w * 24 + 31) and not 31;
  iDst := iDst div 8; //BytesPerScanline
  iSrc := (GetWidth * 24 + 31) and not 31;
  iSrc := iSrc div 8;
}
  // BytesPerScanline
  iDst := FreeImage_GetPitch(DestBmp);
  iSrc := FreeImage_GetPitch(SrcBmp);

  xscale := 1 / (w / FreeImage_GetWidth(SrcBmp));
  yscale := 1 / (h / FreeImage_GetHeight(SrcBmp));

  // X lookup table
  SetLength(lutX, w);
  x1 := 0;
  x2 := trunc(xscale);
  for x := 0 to w - 1 do
  begin
    lutX[x] := x2 - x1;
    x1 := x2;
    x2 := trunc((x + 2) * xscale);
  end;

  // Y lookup table
  SetLength(lutY, h);
  x1 := 0;
  x2 := trunc(yscale);
  for x := 0 to h - 1 do
  begin
    lutY[x] := x2 - x1;
    x1 := x2;
    x2 := trunc((x + 2) * yscale);
  end;

  Dec(w);
  Dec(h);
  RowDest := integer(FreeImage_GetScanLine(DestBmp, 0));
  RowSourceStart := integer(FreeImage_GetScanLine(SrcBmp, 0));
  RowSource := RowSourceStart;

  for y := 0 to h do
  // resampling
  begin
    dy := lutY[y];
    x1 := 0;
    x3 := 0;
    for x := 0 to w do  // loop through row
    begin
      dx:= lutX[x];
      iRed:= 0;
      iGrn:= 0;
      iBlu:= 0;
      RowSource := RowSourceStart;
      for iy := 1 to dy do
      begin
        pt := PRGB24(RowSource + x1);
        for ix := 1 to dx do
        begin
          iRed := iRed + pt.R;
          iGrn := iGrn + pt.G;
          iBlu := iBlu + pt.B;
          inc(pt);
        end;
        RowSource := RowSource + iSrc;
      end;
      iRatio := 65535 div (dx * dy);
      pt1 := PRGB24(RowDest + x3);
      pt1.R := (iRed * iRatio) shr 16;
      pt1.G := (iGrn * iRatio) shr 16;
      pt1.B := (iBlu * iRatio) shr 16;
      x1 := x1 + 3 * dx;
      inc(x3,3);
    end;
    RowDest := RowDest + iDst;
    RowSourceStart := RowSource;
  end; // resampling

  if FreeImage_GetHeight(DestBmp) >= 3 then
  // Sharpening...
  begin
    s1 := integer(FreeImage_GetScanLine(DestBmp, 0));
    iDst := integer(FreeImage_GetScanLine(DestBmp, 1)) - s1;
    ny1 := Integer(s1);
    ny2 := ny1 + iDst;
    ny3 := ny2 + iDst;
    for y := 1 to FreeImage_GetHeight(DestBmp) - 2 do
    begin
      for x := 0 to FreeImage_GetWidth(DestBmp) - 3 do
      begin
        x1 := x * 3;
        x2 := x1 + 3;
        x3 := x1 + 6;

        c1 := pRGB24(ny1 + x1)^;
        c2 := pRGB24(ny1 + x3)^;
        c3 := pRGB24(ny2 + x2)^;
        c4 := pRGB24(ny3 + x1)^;
        c5 := pRGB24(ny3 + x3)^;

        r := (c1.R + c2.R + (c3.R * -12) + c4.R + c5.R) div -8;
        g := (c1.G + c2.G + (c3.G * -12) + c4.G + c5.G) div -8;
        b := (c1.B + c2.B + (c3.B * -12) + c4.B + c5.B) div -8;

        if r < 0 then r := 0 else if r > 255 then r := 255;
        if g < 0 then g := 0 else if g > 255 then g := 255;
        if b < 0 then b := 0 else if b > 255 then b := 255;

        pt1 := pRGB24(ny2 + x2);
        pt1.R := r;
        pt1.G := g;
        pt1.B := b;
      end;
      inc(ny1, iDst);
      inc(ny2, iDst);
      inc(ny3, iDst);
    end;
  end; // sharpening

  if SrcBmp <> FDib then
    FreeImage_Unload(SrcBmp);
  DestBitmap.Replace(DestBmp);
end;

function TFreeBitmap.PasteSubImage(Src: TFreeBitmap; Left, Top,
  Alpha: Integer): Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_Paste(FDib, Src.Dib, Left, Top, Alpha);
    Change;
  end else
    Result := False;
end;

function TFreeBitmap.Replace(NewDib: PFIBITMAP): Boolean;
begin
  Result := False;
  if NewDib = nil then Exit;

  if FDib <> nil then
    FreeImage_Unload(FDib);

  FDib := NewDib;
  Result := True;
  Change;
end;

function TFreeBitmap.Rescale(NewWidth, NewHeight: Integer;
  Filter: TFreeStretchFilter; Dest: TFreeBitmap): Boolean;
const
  cFilter: array [TFreeStretchFilter] of FREE_IMAGE_FILTER = (
    FILTER_BOX,
    FILTER_BICUBIC,
    FILTER_BILINEAR,
    FILTER_BSPLINE,
    FILTER_CATMULLROM,
    FILTER_LANCZOS3
  );
var
  Bpp: Integer;
  DstDib: PFIBITMAP;
begin
  Result := False;

  if FDib <> nil then
  begin
    Bpp := FreeImage_GetBPP(FDib);

    if Bpp < 8 then
      if not ConvertToGrayscale then Exit
    else
    if Bpp = 16 then
    // convert to 24-bit
      if not ConvertTo24Bits then Exit;

    // perform upsampling / downsampling
    DstDib := FreeImage_Rescale(FDib, NewWidth, NewHeight, cFilter[Filter]);
    if Dest = nil then
      Result := Replace(DstDib)
    else
      Result := Dest.Replace(DstDib)
  end
end;

function TFreeBitmap.Rotate(Angle: Double): Boolean;
var
  Bpp: Integer;
  Rotated: PFIBITMAP;
begin
  Result := False;
  if FDib <> nil then
  begin
    Bpp := FreeImage_GetBPP(FDib);
    if (Bpp = 1) or (Bpp >= 8) then
    begin
      Rotated := FreeImage_RotateClassic(FDib, Angle);
      Result := Replace(Rotated);
    end
  end;
end;

function TFreeBitmap.RotateEx(Angle, XShift, YShift, XOrigin,
  YOrigin: Double; UseMask: Boolean): Boolean;
var
  Rotated: PFIBITMAP;
begin
  Result := False;
  if FDib <> nil then
  begin
    if FreeImage_GetBPP(FDib) >= 8 then
    begin
      Rotated := FreeImage_RotateEx(FDib, Angle, XShift, YShift, XOrigin, YOrigin, UseMask);
      Result := Replace(Rotated);
    end
  end;
end;

function TFreeBitmap.Save(const FileName: string; Flag: Integer): Boolean;
var
  fif: FREE_IMAGE_FORMAT;
  CanSave: Boolean;
  ImageType: FREE_IMAGE_TYPE;
  Bpp: Word;
begin
  Result := False;

  // try to guess the file format from the file extension
  fif := FreeImage_GetFIFFromFilename(PChar(Filename));
  if fif <> FIF_UNKNOWN then
  begin
    // check that the dib can be saved in this format
    ImageType := FreeImage_GetImageType(FDib);
    if ImageType = FIT_BITMAP then
    begin
      // standart bitmap type
      Bpp := FreeImage_GetBPP(FDib);
      CanSave := FreeImage_FIFSupportsWriting(fif)
                 and FreeImage_FIFSupportsExportBPP(fif, Bpp);
    end
    else // special bitmap type
      CanSave := FreeImage_FIFSupportsExportType(fif, ImageType);

    if CanSave then
      Result := FreeImage_Save(fif, FDib, PChar(FileName), Flag)
  end
end;

function TFreeBitmap.SaveToHandle(fif: FREE_IMAGE_FORMAT; IO: PFreeImageIO;
  Handle: fi_handle; Flag: Integer): Boolean;
var
  CanSave: Boolean;
  ImageType: FREE_IMAGE_TYPE;
  Bpp: Word;
begin
  Result := False;

  if fif <> FIF_UNKNOWN then
  begin
    // check that the dib can be saved in this format
    ImageType := FreeImage_GetImageType(FDib);
    if ImageType = FIT_BITMAP then
    begin
      // standart bitmap type
      Bpp := FreeImage_GetBPP(FDib);
      CanSave := FreeImage_FIFSupportsWriting(fif)
                 and FreeImage_FIFSupportsExportBPP(fif, Bpp);
    end
    else // special bitmap type
      CanSave := FreeImage_FIFSupportsExportType(fif, ImageType);

    if CanSave then
      Result := FreeImage_SaveToHandle(fif, FDib, IO, Handle, Flag)
  end
end;

function TFreeBitmap.SaveToMemory(fif: FREE_IMAGE_FORMAT;
  MemIO: TFreeMemoryIO; Flag: Integer): Boolean;
var
  CanSave: Boolean;
  ImageType: FREE_IMAGE_TYPE;
  Bpp: Word;
begin
  Result := False;

  if fif <> FIF_UNKNOWN then
  begin
    // check that the dib can be saved in this format
    ImageType := FreeImage_GetImageType(FDib);
    if ImageType = FIT_BITMAP then
    begin
      // standart bitmap type
      Bpp := FreeImage_GetBPP(FDib);
      CanSave := FreeImage_FIFSupportsWriting(fif)
                 and FreeImage_FIFSupportsExportBPP(fif, Bpp);
    end
    else // special bitmap type
      CanSave := FreeImage_FIFSupportsExportType(fif, ImageType);

    if CanSave then
      Result := MemIO.Write(fif, FDib, Flag)
  end
end;

function TFreeBitmap.SetChannel(Bitmap: TFreeBitmap;
  Channel: FREE_IMAGE_COLOR_CHANNEL): Boolean;
begin
  if FDib <> nil then
  begin
    Result := FreeImage_SetChannel(FDib, Bitmap.FDib, Channel);
    Change;
  end
  else
    Result := False
end;

function TFreeBitmap.SetFileBkColor(BkColor: PRGBQuad): Boolean;
begin
  Result := FreeImage_SetBackgroundColor(FDib, BkColor);
  Change;
end;

procedure TFreeBitmap.SetHorizontalResolution(Value: Integer);
var
  bmih: PBitmapInfoHeader;
begin
  bmih := FreeImage_GetInfoHeader(FDib);
  bmih.biXPelsPerMeter := Value * 100;
end;

function TFreeBitmap.SetPixelColor(X, Y: Cardinal;
  Value: PRGBQUAD): Boolean;
begin
  Result := FreeImage_SetPixelColor(FDib, X, Y, Value);
  Change;
end;

function TFreeBitmap.SetPixelIndex(X, Y: Cardinal; Value: PByte): Boolean;
begin
  Result := FreeImage_SetPixelIndex(FDib, X, Y, Value);
  Change;
end;

function TFreeBitmap.SetSize(ImageType: FREE_IMAGE_TYPE; Width, Height,
  Bpp: Integer; RedMask, GreenMask, BlueMask: Cardinal): Boolean;
var
  Pal: PRGBQuad;
  I: Cardinal;
begin
  Result := False;

  if FDib <> nil then
    FreeImage_Unload(FDib);

  FDib := FreeImage_Allocate(Width, Height, Bpp, RedMask, GreenMask, BlueMask);
  if FDib = nil then Exit;

  if ImageType = FIT_BITMAP then
  case Bpp of
    1, 4, 8:
    begin
      Pal := FreeImage_GetPalette(FDib);
      for I := 0 to FreeImage_GetColorsUsed(FDib) - 1 do
      begin
        Pal.rgbBlue := I;
        Pal.rgbGreen := I;
        Pal.rgbRed := I;
        Inc(Pal, SizeOf(RGBQUAD));
      end;
    end;
  end;

  Result := True;
  Change;
end;

procedure TFreeBitmap.SetTransparencyTable(Table: PByte; Count: Integer);
begin
  FreeImage_SetTransparencyTable(FDib, Table, Count);
  Change;
end;

procedure TFreeBitmap.SetVerticalResolution(Value: Integer);
var
  bmih: PBitmapInfoHeader;
begin
  bmih := FreeImage_GetInfoHeader(FDib);
  bmih.biYPelsPerMeter := Value * 100;
end;

function TFreeBitmap.SplitChannels(RedChannel, GreenChannel,
  BlueChannel: TFreeBitmap): Boolean;
begin
  if FDib <> nil then
  begin
    RedChannel.FDib := FreeImage_GetChannel(FDib, FICC_RED);
    GreenChannel.FDib := FreeImage_GetChannel(FDib, FICC_GREEN);
    BlueChannel.FDib := FreeImage_GetChannel(FDib, FICC_BLUE);
    Result := RedChannel.IsValid and GreenChannel.IsValid and BlueChannel.IsValid;
  end
  else
    Result := False  
end;

function TFreeBitmap.Threshold(T: Byte): Boolean;
var
  dib1: PFIBITMAP;
begin
  if FDib <> nil then
  begin
    dib1 := FreeImage_Threshold(FDib, T);
    Result := Replace(dib1);
  end
  else
    Result := False
end;

{ TFreeWinBitmap }

function TFreeWinBitmap.CaptureWindow(ApplicationWindow,
  SelectedWindow: HWND): Boolean;
var
  XScreen, YScreen, XShift, YShift, Width, Height: Integer;
  R: TRect;
  dstDC, srcDC, memDC: HDC;
  BM, oldBM: HBITMAP;
begin
  Result := False;

  // get window size
  GetWindowRect(SelectedWindow, R);

  // check if the window is out of screen or maximized
  XShift := 0;
  YShift := 0;
  XScreen := GetSystemMetrics(SM_CXSCREEN);
  YScreen := GetSystemMetrics(SM_CYSCREEN);
  if R.Right > XScreen then
    R.Right := XScreen;
  if R.Bottom > YScreen then
    R.Bottom := YScreen;
  if R.Left < 0 then
  begin
    XShift := -R.Left;
    R.Left := 0;
  end;
  if R.Top < 0 then
  begin
    YShift := -R.Top;
    R.Top := 0;
  end;

  Width := R.Right - R.Left;
  Height := R.Bottom - R.Top;

  if (Width <= 0) or (Height <= 0) then Exit;

  // hide the application window
  ShowWindow(ApplicationWindow, SW_HIDE);

  // bring the window at the top most level
  SetWindowPos(SelectedWindow, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE or SWP_NOSIZE);

  // give enough time to refresh the window
  Sleep(500);

  // prepare the DCs
  dstDc := GetDC(0);
  srcDC := GetWindowDC(SelectedWindow); //full window (GetDC(SelectedWindow) = clientarea)
  memDC := CreateCompatibleDC(dstDC);

  // copy the screen to the bitmap
  BM := CreateCompatibleBitmap(dstDC, Width, Height);
  oldBM := HBITMAP(SelectObject(memDC, BM));
  BitBlt(memDC, 0, 0, Width, Height, srcDC, XShift, YShift, SRCCOPY);

  // redraw the application window
  ShowWindow(ApplicationWindow, SW_SHOW);

  // restore the position
  SetWindowPos(SelectedWindow, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE or SWP_NOSIZE);
  SetWindowPos(ApplicationWindow, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE or SWP_NOSIZE);

  // convert the HBITMAP to FIBITMAP
  CopyFromBitmap(BM);

  // free objects
  DeleteObject(SelectObject(memDC, oldBM));
  DeleteObject(memDC);

  Result := True;
end;

function TFreeWinBitmap.CopyFromBitmap(HBmp: HBITMAP): Boolean;
var
  bm: BITMAP;
  DC: HDC;
  Success: Integer;
begin
  Result := False;

  if HBmp <> 0 then
  begin
    // get information about the bitmap
    GetObject(HBmp, SizeOf(BITMAP), @bm);

    // create the image
    SetSize(FIT_BITMAP, bm.bmWidth, bm.bmHeight, bm.bmBitsPixel);

    // create the device context for the bitmap
    DC := GetDC(0);

    // copy the pixels
    Success := GetDIBits(DC,                       // handle to DC
                         HBmp,                     // handle to Bitmap
                         0,                        // first scan line
                         FreeImage_GetHeight(Dib), // number of scan lines to copy
                         FreeImage_GetBits(Dib),   // array for bitmap bits
                         FreeImage_GetInfo(Dib^)^, // bitmap data buffer
                         DIB_RGB_COLORS            // RGB
    );

    ReleaseDC(0, DC);

    if Success = 0 then
      raise Exception.Create('Error: GetDIBits failed')
    else
      Result := True;
  end;
end;

function TFreeWinBitmap.CopyFromHandle(HMem: THandle): Boolean;
var
  Data: PByte;
  bmih: PBitmapInfoHeader;
  Palette: PRGBQuad;
  Bits: PByte;
  BitFields: array [0..2] of DWORD;
  MaskSize: Longint;
begin
  Result := False;
  Palette := nil;
  BitFields[0] := 0; BitFields[1] := 0; BitFields[2] := 0;

  // get a pointer to the bitmap
  Data := GlobalLock(HMem);

  // get a pointer to the bitmap header
  bmih := PBitmapInfoHeader(Data);

  // get a pointer to the palette
  if bmih.biBitCount < 16 then
  begin
    Palette := PRGBQUAD(bmih);
    Inc(PByte(Palette), SizeOf(BITMAPINFOHEADER));
  end;

  // get a pointer to the pixels
  Bits := PByte(bmih);
  Inc(Bits, SizeOf(BITMAPINFOHEADER) + SizeOF(RGBQUAD) * bmih.biClrUsed);

  if bmih.biCompression = BI_BITFIELDS then
  begin
    // take into account the color masks that specify the red, green and blue
    // components (16- and 32-bit)
    MaskSize := 3 * SizeOf(DWORD);
    CopyMemory(@BitFields[0], Bits, MaskSize);
    Inc(Bits, MaskSize);
  end;

  if Data <> nil then
  begin
    // allocate a new FIBITMAP
    if not SetSize(FIT_BITMAP, bmih.biWidth, bmih.biHeight, bmih.biBitCount,
                   BitFields[2], BitFields[1], BitFields[0]) then
    begin
      GlobalUnlock(HMem);
      Exit;
    end;

    // copy the bitmap header
    CopyMemory(FreeImage_GetInfoHeader(Dib), bmih, SizeOf(BITMAPINFOHEADER));

    // copy the palette
    CopyMemory(FreeImage_GetPalette(Dib), Palette, bmih.biClrUsed * SizeOf(RGBQUAD));

    // copy the bitmap
    CopyMemory(FreeImage_GetBits(Dib), Bits, FreeImage_GetPitch(Dib) * FreeImage_GetHeight(Dib));

    GlobalUnlock(HMem);
  end;
end;

function TFreeWinBitmap.CopyToBitmapH: HBITMAP;
var DC : HDC;
begin
  Result:=0;
  if IsValid then
  begin
    DC:=GetDC(0);
    Result:=CreateDIBitmap(DC,
                           FreeImage_GetInfoHeader(Dib)^,
                           CBM_INIT,
                           PAnsiChar(FreeImage_GetBits(Dib)),
                           FreeImage_GetInfo(Dib^)^,
                           DIB_RGB_COLORS);
    ReleaseDC(0,DC);
  end;
end;

function TFreeWinBitmap.CopyToClipBoard(NewOwner: HWND): Boolean;
var
  HDib: THandle;
begin
  Result := False;
  HDib := CopyToHandle;

  if OpenClipboard(NewOwner) and EmptyClipboard then
  begin
    if SetClipboardData(CF_DIB, HDib) = 0 then
    begin
      MessageBox(NewOwner, 'Unable to set clipboard data', 'FreeImage', MB_ICONERROR);
      CloseClipboard;
      Exit;
    end;
  end;
  CloseClipboard;
  Result := True;
end;

function TFreeWinBitmap.CopyToHandle: THandle;
var
  DibSize: Longint;
  ADib, pdib: PByte;
  bmih: PBITMAPINFOHEADER;
  Pal: PRGBQuad;
  Bits: PByte;
begin
  Result := 0;
  if IsValid then
  begin
    // get equivalent DIB size
    DibSize := SizeOf(BITMAPINFOHEADER);
    Inc(DibSize, FreeImage_GetColorsUsed(Dib) * SizeOf(RGBQUAD));
    Inc(DibSize, FreeImage_GetPitch(Dib) * FreeImage_GetHeight(Dib));

    // allocate a DIB
    Result := GlobalAlloc(GHND, DibSize);
    ADib := GlobalLock(Result);

    FillChar(Result, DibSize, 0);

    pdib := ADib;

    // copy the BITMAPINFOHEADER
    bmih := FreeImage_GetInfoHeader(Dib);
    CopyMemory(pdib, bmih, SizeOf(BITMAPINFOHEADER));
    Inc(pdib, SizeOf(BITMAPINFOHEADER));

    // copy the palette
    Pal := FreeImage_GetPalette(Dib);
    CopyMemory(pdib, Pal, FreeImage_GetColorsUsed(Dib) * SizeOf(RGBQUAD));
    Inc(pdib, FreeImage_GetColorsUsed(Dib) * SizeOf(RGBQUAD));

    // copy the bitmap
    Bits := FreeImage_GetBits(Dib);
    CopyMemory(pdib, Bits, FreeImage_GetPitch(Dib) * FreeImage_GetHeight(Dib));

    GlobalUnlock(Result);
  end;
end;

constructor TFreeWinBitmap.Create(ImageType: FREE_IMAGE_TYPE; Width,
  Height, Bpp: Integer);
begin
  inherited Create(ImageType, Width, Height, Bpp);

  FDisplayDib := nil;
  FDeleteMe := False;
end;

destructor TFreeWinBitmap.Destroy;
begin
  if FDeleteMe then
    FreeImage_Unload(FDisplayDib);
  inherited;
end;

procedure TFreeWinBitmap.Draw(DC: HDC; Rect: TRect);
begin
  DrawEx(DC, Rect);
end;

procedure TFreeWinBitmap.DrawEx(DC: HDC; Rect: TRect; UseFileBkg: Boolean;
  AppBkColor: PRGBQuad; Bg: PFIBITMAP);
var
  ImageType: FREE_IMAGE_TYPE;
  HasBackground, Transparent: Boolean;
  DibDouble: PFIBITMAP;
begin
  if not IsValid then Exit;
  
  // convert to standart bitmap if needed
  if FDeleteMe then
  begin
    FreeImage_Unload(FDisplayDib);
    FDisplayDib := nil;
    FDeleteMe := False;
  end;

  ImageType := FreeImage_GetImageType(FDib);
  if ImageType = FIT_BITMAP then
  begin
    HasBackground := FreeImage_HasBackgroundColor(Dib);
    Transparent := FreeImage_IsTransparent(Dib);

    if not Transparent and not HasBackground then
      // copy pointer
      FDisplayDib := Dib
    else
    begin
      // create the transparent / alpha blended image
      FDisplayDib := FreeImage_Composite(Dib, UseFileBkg, AppBkColor, Bg);
      // remember to delete FDisplayDib
      FDeleteMe := True;
    end
  end
  else
  begin
    // convert to standart dib for display
    if ImageType <> FIT_COMPLEX then
      FDisplayDib := FreeImage_ConvertToStandardType(Dib, True)
    else
    begin
      // convert to type FIT_DOUBLE
      DibDouble := FreeImage_GetComplexChannel(Dib, FICC_MAG);
      FDisplayDib := FreeImage_ConvertToStandardType(DibDouble, True);
      // free image of type FIT_DOUBLE
      FreeImage_Unload(DibDouble);
    end;
    // remember to delete FDisplayDib
    FDeleteMe := True;
  end;


  // Draw the DIB
  SetStretchBltMode(DC, COLORONCOLOR);
  StretchDIBits(DC, Rect.Left, Rect.Top,
    Rect.Right - Rect.Left, Rect.Bottom - Rect.Top,
    0, 0, FreeImage_GetWidth(FDisplayDib), FreeImage_GetHeight(FDisplayDib),
    FreeImage_GetBits(FDisplayDib), FreeImage_GetInfo(FDisplayDib^)^, DIB_RGB_COLORS, SRCCOPY);
end;

function TFreeWinBitmap.PasteFromClipBoard: Boolean;
var
  HDib: THandle;
begin
  Result := False;
  if not IsClipboardFormatAvailable(CF_DIB) then Exit;

  if OpenClipboard(0) then
  begin
    HDib := GetClipboardData(CF_DIB);
    CopyFromHandle(HDib);
    Result := True;
  end;
  CloseClipboard;
end;

{ TFreeMultiBitmap }

procedure TFreeMultiBitmap.AppendPage(Bitmap: TFreeBitmap);
begin
  if IsValid then
    FreeImage_AppendPage(FMPage, Bitmap.FDib);
end;

function TFreeMultiBitmap.Close(Flags: Integer): Boolean;
begin
  Result := FreeImage_CloseMultiBitmap(FMPage, Flags);
  FMPage := nil;
end;

constructor TFreeMultiBitmap.Create(KeepCacheInMemory: Boolean);
begin
  FMemoryCache := KeepCacheInMemory;
end;

procedure TFreeMultiBitmap.DeletePage(Page: Integer);
begin
  if IsValid then
    FreeImage_DeletePage(FMPage, Page);
end;

destructor TFreeMultiBitmap.Destroy;
begin
  if FMPage <> nil then Close;
  inherited;
end;

function TFreeMultiBitmap.GetLockedPageNumbers(var Pages,
  Count: Integer): Boolean;
begin
  Result := False;
  if not IsValid then Exit;
  Result := FreeImage_GetLockedPageNumbers(FMPage, Pages, Count)
end;

function TFreeMultiBitmap.GetPageCount: Integer;
begin
  Result := 0;
  if IsValid then
    Result := FreeImage_GetPageCount(FMPage)
end;

procedure TFreeMultiBitmap.InsertPage(Page: Integer; Bitmap: TFreeBitmap);
begin
  if IsValid then
    FreeImage_InsertPage(FMPage, Page, Bitmap.FDib);
end;

function TFreeMultiBitmap.IsValid: Boolean;
begin
  Result := FMPage <> nil
end;

procedure TFreeMultiBitmap.LockPage(Page: Integer; DestBitmap: TFreeBitmap);
begin
  if not IsValid then Exit;

  if Assigned(DestBitmap) then
  begin
    DestBitmap.Replace(FreeImage_LockPage(FMPage, Page));
  end;
end;

function TFreeMultiBitmap.MovePage(Target, Source: Integer): Boolean;
begin
  Result := False;
  if not IsValid then Exit;
  Result := FreeImage_MovePage(FMPage, Target, Source);
end;

function TFreeMultiBitmap.Open(const FileName: string; CreateNew,
  ReadOnly: Boolean): Boolean;
var
  fif: FREE_IMAGE_FORMAT;
begin
  Result := False;

  // try to guess the file format from the filename
  fif := FreeImage_GetFIFFromFilename(PChar(FileName));

  // check for supported file types
  if (fif <> FIF_TIFF) and (fif <> FIF_ICO) then
    Exit;

  // open the stream
  FMPage := FreeImage_OpenMultiBitmap(fif, PChar(FileName), CreateNew, ReadOnly, FMemoryCache);

  Result := FMPage <> nil;  
end;

procedure TFreeMultiBitmap.UnlockPage(Bitmap: TFreeBitmap;
  Changed: Boolean);
begin
  if IsValid then
  begin
    FreeImage_UnlockPage(FMPage, Bitmap.FDib, Changed);
    // clear the image so that it becomes invalid.
    // don't use Bitmap.Clear method because it calls FreeImage_Unload
    // just clear the pointer
    Bitmap.FDib := nil;
    Bitmap.Change;
  end;
end;

{ TFreeMemoryIO }

function TFreeMemoryIO.Acquire(var Data: PByte;
  var SizeInBytes: DWORD): Boolean;
begin
  Result := FreeImage_AcquireMemory(FHMem, Data, SizeInBytes);
end;

constructor TFreeMemoryIO.Create(Data: PByte; SizeInBytes: DWORD);
begin
  FHMem := FreeImage_OpenMemory(Data, SizeInBytes);
end;

destructor TFreeMemoryIO.Destroy;
begin
  FreeImage_CloseMemory(FHMem);
  inherited;
end;

function TFreeMemoryIO.GetFileType: FREE_IMAGE_FORMAT;
begin
  Result := FreeImage_GetFileTypeFromMemory(FHMem)
end;

function TFreeMemoryIO.IsValid: Boolean;
begin
  Result := FHMem <> nil
end;

function TFreeMemoryIO.Read(fif: FREE_IMAGE_FORMAT;
  Flag: Integer): PFIBITMAP;
begin
  Result := FreeImage_LoadFromMemory(fif, FHMem, Flag)
end;

function TFreeMemoryIO.Seek(Offset, Origin: Integer): Boolean;
begin
  Result := FreeImage_SeekMemory(FHMem, Offset, Origin)
end;

function TFreeMemoryIO.Tell: Longint;
begin
  Result := FreeImage_TellMemory(FHMem)
end;

function TFreeMemoryIO.Write(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP;
  Flag: Integer): Boolean;
begin
  Result := FreeImage_SaveToMemory(fif, dib, FHMem, Flag)
end;

end.
