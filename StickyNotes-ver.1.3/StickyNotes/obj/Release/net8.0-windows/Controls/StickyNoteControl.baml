   M S B A M L   `   `   `  ����   StickyNotes,    &StickyNotes.Controls.StickyNoteControl   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � WindowStyle$
  None=� � AllowsTransparency$
 True��
������  � Topmost$
 True�� � SizeToContent$ WidthAndHeight=�.���� -    �� CornerRadius$ 8�����#666$��1q� �� Padding$ 10q� J� 	CacheMode-  'System.Windows.Media.CacheModeConverter$ BitmapCache ���� *BackgroundColor+   ColorToBrushConverter �� 	Converter# [   �� RelativeSource �� 	 �� AncestorType
	 Window
 � ContextMenu
 �� .@�m� -   $��删除��m� -    m�Name$ PinMenuItem��$��固定到窗口��m� -   $ UnpinMenuItem��$��取消固定��$y�	Collapsed=� � Triggers :�  :� RoutedEvent$ 
MouseEnter��.[��� .���� .��W�  �� TargetProperty$ Opacity�	 W� To 0.9 g� Duration$ 0:0:0.2D�:� $ 
MouseLeave��.[��� .���� .��W� $ Opacity� 1$ 0:0:0.2D� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     Color$ 	#40000000��   Opacity$	 0.6Q�   
BlurRadius$ 12Q�   ShadowDepth$ 4Q�z�T� -   MoveTransform��._��� .+��� -    ��Name$ EditBox��$y�	Collapsed=� �� AcceptsReturn$
 True�� �� TextWrapping$
 Wrap=����� *
FontSize+���� *NoteContent+ �� UpdateSourceTrigger$ PropertyChanged=��� -    ��Name$ DisplayTextBlock��  NoteTextStyle#��[ $
��Wrap=����� *
FontSize+���� *NoteContent+