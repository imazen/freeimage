unit MainForm;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Menus, FreeBitmap, ComCtrls;

type
  TfwbMainForm = class(TForm)
    MainMenu: TMainMenu;
    mnuFile: TMenuItem;
    mnuFileOpen: TMenuItem;
    mnuExit: TMenuItem;
    OD: TOpenDialog;
    StatusBar: TStatusBar;
    mnuImage: TMenuItem;
    mnuImageFlip: TMenuItem;
    mnuFlipHorz: TMenuItem;
    mnuFlipVert: TMenuItem;
    mnuConvert: TMenuItem;
    mnuTo8Bits: TMenuItem;
    mnuTo16Bits555: TMenuItem;
    mnuTo16Bits565: TMenuItem;
    mnuTo24Bits: TMenuItem;
    mnuTo32Bits: TMenuItem;
    mnuDither: TMenuItem;
    mnuQuantize: TMenuItem;
    mnuGrayScale: TMenuItem;
    mnuRotate: TMenuItem;
    mnuClockwise: TMenuItem;
    mnuAntiClockwise: TMenuItem;
    mnuInvert: TMenuItem;
    mnuClear: TMenuItem;
    mnuTo4Bits: TMenuItem;
    procedure FormDestroy(Sender: TObject);
    procedure FormPaint(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure mnuExitClick(Sender: TObject);
    procedure mnuFileOpenClick(Sender: TObject);
    procedure FormResize(Sender: TObject);
    procedure mnuFlipHorzClick(Sender: TObject);
  private
    FBitmap: TFreeWinBitmap;
    procedure WMEraseBkgnd(var Message: TMessage); message WM_ERASEBKGND;
  public
    { Public declarations }
  end;

var
  fwbMainForm: TfwbMainForm;

implementation

{$R *.dfm}

uses
  FreeUtils, FreeImage;

procedure TfwbMainForm.FormDestroy(Sender: TObject);
begin
  if Assigned(FBitmap) then
    FBitmap.Free;
end;

procedure TfwbMainForm.FormPaint(Sender: TObject);
begin
  if FBitmap.IsValid then
    FBitmap.Draw(Canvas.Handle, ClientRect)
  else
  begin
    Canvas.Brush.Color := clBtnFace;
    Canvas.FillRect(ClientRect);
  end
end;

procedure TfwbMainForm.FormCreate(Sender: TObject);
begin
  FBitmap := TFreeWinBitmap.Create;

  mnuImage.Enabled := FBitmap.IsValid;
  OD.Filter := FIU_GetAllFilters;
end;

procedure TfwbMainForm.mnuExitClick(Sender: TObject);
begin
  Close;
end;

const
  cColorType: array [FREE_IMAGE_COLOR_TYPE] of string = (
    'MinIsWhite',
    'MinIsBlack',
    'RGB',
    'Palette',
    'RGBAlpha',
    'CMYK'
  );

procedure TfwbMainForm.mnuFileOpenClick(Sender: TObject);
var
  t: Cardinal;
begin
  if OD.Execute then
  begin
    t := GetTickCount;
    FBitmap.Load(OD.FileName);
    t := GetTickCount - t;
    mnuImage.Enabled := FBitmap.IsValid;
    StatusBar.Panels[0].Text := 'Loaded in ' + IntToStr(t) + 'msec.';
    StatusBar.Panels[1].Text := Format('%dx%d', [FBitmap.GetWidth, FBitmap.GetHeight]);
    StatusBar.Panels[2].Text := 'Color type: ' + cColorType[FBitmap.GetColorType];
    Invalidate;
  end;
end;

procedure TfwbMainForm.FormResize(Sender: TObject);
begin
  Invalidate
end;

procedure TfwbMainForm.WMEraseBkgnd(var Message: TMessage);
begin
  Message.Result := 1;
end;

procedure TfwbMainForm.mnuFlipHorzClick(Sender: TObject);
begin
  with FBitmap do
  if Sender = mnuFlipHorz then
    FLipHorizontal else
  if Sender = mnuFlipVert then
    FlipVertical else
  if Sender = mnuTo4Bits then
    ConvertTo4Bits else
  if Sender = mnuTo8Bits then
    ConvertTo8Bits else
  if Sender = mnuTo16Bits555 then
    ConvertTo16Bits555 else
  if Sender = mnuTo16Bits565 then
    ConvertTo16Bits565 else
  if Sender = mnuTo24Bits then
    ConvertTo24Bits else
  if Sender = mnuTo32Bits then
    ConvertTo32Bits else
  if Sender = mnuDither then
    Dither(FID_FS) else
  if Sender = mnuQuantize then
    ColorQuantize(FIQ_WUQUANT) else
  if Sender = mnuGrayScale then
    ConvertToGrayscale else
  if Sender = mnuClockwise then
    Rotate(-90) else
  if Sender = mnuAntiClockwise then
    Rotate(90) else
  if Sender = mnuInvert then
    Invert else
  if Sender = mnuClear then
    Clear;
  StatusBar.Panels[2].Text := 'Color type: ' + cColorType[FBitmap.GetColorType];
  Invalidate;
end;

end.
