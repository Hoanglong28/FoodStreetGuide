using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace doanC_.Services.Localization;

public static class AppResources
{
    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
    {
      {
            "vi", new Dictionary<string, string>
          {
   { "SearchPlaceholder", "Tìm ki?m..." },
     { "AllCategories", "T?t c?" },
                { "Restaurant", "Quán ?n" },
      { "Location", "??a ?i?m" },
   { "History", "L?ch s?" },
      { "Error", "L?i" },
              { "OK", "OK" },
      { "NotFound", "Không tìm th?y thông tin" },
         { "CannotLoadData", "Không th? t?i d? li?u" },
          { "Cancel", "H?y" },
    { "Settings", "Cài ??t" },
   { "Language", "Ngôn ng?" },
    { "Vietnamese", "Ti?ng Vi?t" },
         { "English", "Ti?ng Anh" },
       { "Chinese", "Ti?ng Trung" },
        { "Play", "Phát" },
    { "Stop", "D?ng" },
        { "TabMap", "B?n ??" },
       { "TabPoi", "?i?m" },
           { "TabQr", "QR" },
 { "TabSettings", "Cài ??t" },
  { "PoiDetailTitle", "Chi ti?t ?i?m tham quan" },
         { "OpenStatus", "?ang m? c?a" },
                { "DirectionsButton", "Ch? ???ng" },
       { "AudioButton", "Audio" },
        { "AudioPlayerTitle", "Thuy?t minh âm thanh" },
         { "PlayButton", "Phát" },
       { "AudioLanguagePickerTitle", "Ch?n ngôn ng?" },
     { "LanguageSettingsSection", "NGÔN NG? & GI?NG ??C" },
     { "LanguageLabel", "Ngôn ng?" },
     { "VoiceTTSLabel", "Gi?ng TTS" },
            { "GpsGeofenceSection", "GPS & GEOFENCE" },
 { "RadiusActivationLabel", "Bán kính kích ho?t" },
     { "BackgroundTrackingLabel", "Theo dõi n?n" },
      { "BatterySaveLabel", "Ti?t ki?m pin" },
     { "OfflineContentSection", "N?I DUNG OFFLINE" },
        { "DownloadOfflineLabel", "T?i gói offline" },
           { "OfflinePackageInfo", "Ph? Lê Thánh Tôn · 24MB" }
            }
   },
   {
          "en", new Dictionary<string, string>
      {
      { "SearchPlaceholder", "Search..." },
       { "AllCategories", "All" },
    { "Restaurant", "Restaurant" },
            { "Location", "Location" },
   { "History", "History" },
     { "Error", "Error" },
          { "OK", "OK" },
                { "NotFound", "Not found" },
           { "CannotLoadData", "Cannot load data" },
            { "Cancel", "Cancel" },
           { "Settings", "Settings" },
    { "Language", "Language" },
    { "Vietnamese", "Vietnamese" },
           { "English", "English" },
         { "Chinese", "Chinese" },
             { "Play", "Play" },
                { "Stop", "Stop" },
      { "TabMap", "Map" },
     { "TabPoi", "Points" },
         { "TabQr", "QR" },
    { "TabSettings", "Settings" },
         { "PoiDetailTitle", "POI Details" },
     { "OpenStatus", "Now Open" },
                { "DirectionsButton", "Directions" },
    { "AudioButton", "Audio" },
         { "AudioPlayerTitle", "Audio Commentary" },
          { "PlayButton", "Play" },
                { "AudioLanguagePickerTitle", "Select Language" },
        { "LanguageSettingsSection", "LANGUAGE & VOICE" },
              { "LanguageLabel", "Language" },
     { "VoiceTTSLabel", "TTS Voice" },
        { "GpsGeofenceSection", "GPS & GEOFENCE" },
        { "RadiusActivationLabel", "Trigger Radius" },
             { "BackgroundTrackingLabel", "Background Tracking" },
    { "BatterySaveLabel", "Battery Save" },
          { "OfflineContentSection", "OFFLINE CONTENT" },
    { "DownloadOfflineLabel", "Download Offline" },
    { "OfflinePackageInfo", "Le Thanh Ton Street · 24MB" }
            }
 },
        {
            "zh", new Dictionary<string, string>
            {
          { "SearchPlaceholder", "??..." },
                { "AllCategories", "??" },
             { "Restaurant", "??" },
    { "Location", "??" },
      { "History", "??" },
          { "Error", "??" },
        { "OK", "??" },
         { "NotFound", "???" },
   { "CannotLoadData", "??????" },
     { "Cancel", "??" },
                { "Settings", "??" },
    { "Language", "??" },
          { "Vietnamese", "???" },
        { "English", "??" },
                { "Chinese", "??" },
       { "Play", "??" },
      { "Stop", "??" },
 { "TabMap", "??" },
      { "TabPoi", "??" },
              { "TabQr", "???" },
      { "TabSettings", "??" },
     { "PoiDetailTitle", "?????" },
  { "OpenStatus", "????" },
  { "DirectionsButton", "??" },
 { "AudioButton", "??" },
      { "AudioPlayerTitle", "????" },
          { "PlayButton", "??" },
                { "AudioLanguagePickerTitle", "????" },
     { "LanguageSettingsSection", "?????" },
            { "LanguageLabel", "??" },
      { "VoiceTTSLabel", "???????" },
                { "GpsGeofenceSection", "GPS?????" },
     { "RadiusActivationLabel", "????" },
         { "BackgroundTrackingLabel", "????" },
    { "BatterySaveLabel", "??" },
                { "OfflineContentSection", "????" },
            { "DownloadOfflineLabel", "????" },
      { "OfflinePackageInfo", "????·24MB" }
          }
        },
{
            "fr", new Dictionary<string, string>
            {
    { "SearchPlaceholder", "Rechercher..." },
        { "AllCategories", "Tous" },
            { "Restaurant", "Restaurant" },
                { "Location", "Lieu" },
     { "History", "Histoire" },
     { "Error", "Erreur" },
    { "OK", "OK" },
      { "NotFound", "Non trouvé" },
   { "CannotLoadData", "Impossible de charger les données" },
                { "Cancel", "Annuler" },
              { "Settings", "Paramètres" },
  { "Language", "Langue" },
       { "Vietnamese", "Vietnamien" },
       { "English", "Anglais" },
    { "Chinese", "Chinois" },
       { "Play", "Lire" },
         { "Stop", "Arrêter" },
     { "TabMap", "Carte" },
       { "TabPoi", "Points" },
  { "TabQr", "QR" },
     { "TabSettings", "Paramètres" },
           { "PoiDetailTitle", "Détails du point d'intérêt" },
       { "OpenStatus", "Maintenant ouvert" },
                { "DirectionsButton", "Itinéraires" },
       { "AudioButton", "Audio" },
            { "AudioPlayerTitle", "Commentaire audio" },
           { "PlayButton", "Lire" },
                { "AudioLanguagePickerTitle", "Sélectionner la langue" },
   { "LanguageSettingsSection", "LANGUE & VOIX" },
          { "LanguageLabel", "Langue" },
       { "VoiceTTSLabel", "Voix TTS" },
     { "GpsGeofenceSection", "GPS & GEOFENCE" },
       { "RadiusActivationLabel", "Rayon de déclenchement" },
  { "BackgroundTrackingLabel", "Suivi en arrière-plan" },
          { "BatterySaveLabel", "Économie de batterie" },
     { "OfflineContentSection", "CONTENU HORS LIGNE" },
            { "DownloadOfflineLabel", "Télécharger hors ligne" },
    { "OfflinePackageInfo", "Rue Le Thanh Ton · 24MB" }
            }
        },
        {
     "es", new Dictionary<string, string>
   {
          { "SearchPlaceholder", "Buscar..." },
                { "AllCategories", "Todos" },
       { "Restaurant", "Restaurante" },
        { "Location", "Lugar" },
        { "History", "Historia" },
 { "Error", "Error" },
     { "OK", "OK" },
      { "NotFound", "No encontrado" },
       { "CannotLoadData", "No se pueden cargar los datos" },
      { "Cancel", "Cancelar" },
             { "Settings", "Configuración" },
          { "Language", "Idioma" },
 { "Vietnamese", "Vietnamita" },
          { "English", "Inglés" },
          { "Chinese", "Chino" },
       { "Play", "Reproducir" },
        { "Stop", "Detener" },
     { "TabMap", "Mapa" },
      { "TabPoi", "Lugares" },
     { "TabQr", "QR" },
              { "TabSettings", "Configuración" },
       { "PoiDetailTitle", "Detalles del punto de interés" },
    { "OpenStatus", "Abierto ahora" },
          { "DirectionsButton", "Direcciones" },
      { "AudioButton", "Audio" },
    { "AudioPlayerTitle", "Comentario de audio" },
 { "PlayButton", "Reproducir" },
                { "AudioLanguagePickerTitle", "Seleccionar idioma" },
       { "LanguageSettingsSection", "IDIOMA Y VOZ" },
            { "LanguageLabel", "Idioma" },
        { "VoiceTTSLabel", "Voz TTS" },
       { "GpsGeofenceSection", "GPS Y GEOFENCE" },
                { "RadiusActivationLabel", "Radio de activación" },
      { "BackgroundTrackingLabel", "Seguimiento en segundo plano" },
     { "BatterySaveLabel", "Ahorro de batería" },
          { "OfflineContentSection", "CONTENIDO SIN CONEXIÓN" },
     { "DownloadOfflineLabel", "Descargar sin conexión" },
      { "OfflinePackageInfo", "Calle Le Thanh Ton · 24MB" }
       }
        },
     {
   "ja", new Dictionary<string, string>
  {
      { "SearchPlaceholder", "??..." },
    { "AllCategories", "???" },
         { "Restaurant", "?????" },
         { "Location", "??" },
         { "History", "??" },
                { "Error", "???" },
                { "OK", "OK" },
  { "NotFound", "???????" },
                { "CannotLoadData", "???????????" },
         { "Cancel", "?????" },
 { "Settings", "??" },
      { "Language", "??" },
           { "Vietnamese", "?????" },
  { "English", "??" },
 { "Chinese", "???" },
                { "Play", "??" },
       { "Stop", "??" },
          { "TabMap", "??" },
  { "TabPoi", "??" },
          { "TabQr", "QR" },
         { "TabSettings", "??" },
 { "PoiDetailTitle", "???????" },
      { "OpenStatus", "????" },
                { "DirectionsButton", "??" },
     { "AudioButton", "?????" },
            { "AudioPlayerTitle", "????" },
          { "PlayButton", "??" },
           { "AudioLanguagePickerTitle", "?????" },
    { "LanguageSettingsSection", "?????" },
    { "LanguageLabel", "??" },
  { "VoiceTTSLabel", "??????????" },
                { "GpsGeofenceSection", "GPS???????" },
          { "RadiusActivationLabel", "??????" },
 { "BackgroundTrackingLabel", "??????????" },
    { "BatterySaveLabel", "???????" },
 { "OfflineContentSection", "??????????" },
                { "DownloadOfflineLabel", "????????????" },
     { "OfflinePackageInfo", "?????·24MB" }
            }
      },
        {
   "ko", new Dictionary<string, string>
    {
          { "SearchPlaceholder", "??..." },
  { "AllCategories", "??" },
                { "Restaurant", "??" },
             { "Location", "??" },
                { "History", "??" },
      { "Error", "??" },
              { "OK", "??" },
     { "NotFound", "?? ? ??" },
{ "CannotLoadData", "???? ??? ? ??" },
             { "Cancel", "??" },
      { "Settings", "??" },
         { "Language", "??" },
      { "Vietnamese", "????" },
      { "English", "??" },
        { "Chinese", "???" },
            { "Play", "??" },
     { "Stop", "??" },
              { "TabMap", "??" },
  { "TabPoi", "??" },
        { "TabQr", "QR" },
                { "TabSettings", "??" },
     { "PoiDetailTitle", "?? ?? ?? ??" },
     { "OpenStatus", "?? ?? ?" },
    { "DirectionsButton", "? ??" },
      { "AudioButton", "???" },
       { "AudioPlayerTitle", "??? ??" },
        { "PlayButton", "??" },
    { "AudioLanguagePickerTitle", "?? ??" },
     { "LanguageSettingsSection", "?? ? ??" },
    { "LanguageLabel", "??" },
{ "VoiceTTSLabel", "??? ?? ?? ??" },
         { "GpsGeofenceSection", "GPS ? ????" },
  { "RadiusActivationLabel", "??? ??" },
                { "BackgroundTrackingLabel", "????? ??" },
        { "BatterySaveLabel", "??? ??" },
                { "OfflineContentSection", "???? ???" },
  { "DownloadOfflineLabel", "???? ????" },
  { "OfflinePackageInfo", "?? ??·24MB" }
            }
        }
    };

    private static string _currentLanguage = "vi";

    public static void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
            languageCode = "vi";

      _currentLanguage = languageCode;
        Debug.WriteLine($"[AppResources] ?? Language set to: {languageCode}");
    }

    public static string GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;

   if (Translations.TryGetValue(_currentLanguage, out var langDict))
        {
        if (langDict.TryGetValue(key, out var value))
        return value;
        }

        if (Translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enValue))
       return enValue;

        Debug.WriteLine($"[AppResources] ?? Translation key not found: {key}");
        return key;
    }

    public static string GetCurrentLanguage() => _currentLanguage;
}
