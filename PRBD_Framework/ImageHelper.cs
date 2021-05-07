using System;
using System.Diagnostics;
using System.IO;

namespace PRBD_Framework {
    /// <summary>
    /// Cette classe permet de gérer l'état d'un fichier (a priori une image, mais pas uniquement)
    /// lié à un champ dans la base de données, et notamment le fait qu'on peut charger une nouvelle
    /// version, puis décider d'annuler ou de confirmer le chargement (voir les différents états).
    /// </summary>
    public class ImageHelper {

        public enum ImageHelperState {
            Empty,              // Il n'y a pas de fichier associé
            LoadedAfterEmpty,   // Un fichier a été chargé alors qu'il n'y en avait pas et on est
                                // en attente de confirmation
            LoadedAfterSaved,   // Un fichier a été chargé alors qu'il y en avait déjà un et on est
                                // en attente de confirmation
            Saved,              // Le fichier chargé est confirmé et donc sauvé à l'emplacement choisi
                                // et avec le nom choisi
            ClearedAfterEmpty,  // Le fichier associé va être supprimé ; en attente de confirmation
            ClearedAfterSaved,  // Le fichier associé va être supprimé ; en attente de confirmation
        }

        public ImageHelperState State { get; private set; }

        private string basePath = null;                     // Le chemin de base où sont stockés les fichiers
        private string currentFile = null;                  // Le fichier sauvé courant
        private string tempFile = null;                     // Le fichier temporaire créé en cas de chargement en
                                                            // attendant la confirmation

        /// <summary>
        /// Le fichier qui doit être pris en compte dans l'état actuel. Si on est dans un des états temporaires
        /// c'est le fichier temporaire qui est retourné, sinon, c'est le fichier définitif ou null en fonction de l'état.
        /// </summary>
        public string CurrentFile {
            get {
                switch (State) {
                    case ImageHelperState.Empty:
                    case ImageHelperState.Saved:
                        return currentFile;
                    default:
                        return tempFile;
                }
            }
        }

        public bool IsTransitoryState {
            get => State == ImageHelperState.ClearedAfterEmpty ||
                State == ImageHelperState.ClearedAfterSaved ||
                State == ImageHelperState.LoadedAfterEmpty ||
                State == ImageHelperState.LoadedAfterSaved;
        }

        public ImageHelper(string basePath, string currentFile = null) {
            this.basePath = basePath;
            this.currentFile = currentFile;
            State = currentFile == null ? ImageHelperState.Empty : ImageHelperState.Saved;
        }

        public void Load(string newFilePath) {
            if (newFilePath == null || !File.Exists(newFilePath)) {
                Debug.Assert(false, $"Load: bad file path '{newFilePath}'");
            }
            switch (State) {
                case ImageHelperState.ClearedAfterEmpty:
                case ImageHelperState.LoadedAfterEmpty:
                case ImageHelperState.Empty:
                    DeleteFile(tempFile);
                    CreateTempFile(newFilePath);
                    State = ImageHelperState.LoadedAfterEmpty;
                    break;
                case ImageHelperState.ClearedAfterSaved:
                case ImageHelperState.LoadedAfterSaved:
                case ImageHelperState.Saved:
                    DeleteFile(tempFile);
                    CreateTempFile(newFilePath);
                    State = ImageHelperState.LoadedAfterSaved;
                    break;
                default:
                    Console.WriteLine($"Load: bad action for state {State}");
                    break;
            }
        }

        public void Cancel() {
            switch (State) {
                case ImageHelperState.LoadedAfterEmpty:
                    DeleteFile(tempFile);
                    State = ImageHelperState.Empty;
                    break;
                case ImageHelperState.LoadedAfterSaved:
                    DeleteFile(tempFile);
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.ClearedAfterSaved:
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.ClearedAfterEmpty:
                    State = ImageHelperState.Empty;
                    break;
                default:
                    Console.WriteLine($"Cancel: bad action for state {State}");
                    break;
            }
        }

        // On passe en paramètre le nom que doit avoir le fichier définitif, mais sans extension
        // (on réutilise l'extension du fichier temporaire). On ne connait le nom définitif 
        // du fichier qu'au moment de la confirmation.
        public void Confirm(string targetFileWithoutExtension) {
            switch (State) {
                case ImageHelperState.LoadedAfterEmpty:
                    RenameTempFile(targetFileWithoutExtension);
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.LoadedAfterSaved:
                    DeleteFile(currentFile);
                    RenameTempFile(targetFileWithoutExtension);
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.ClearedAfterSaved:
                    DeleteFile(currentFile);
                    currentFile = null;
                    State = ImageHelperState.Empty;
                    break;
                case ImageHelperState.ClearedAfterEmpty:
                    State = ImageHelperState.Empty;
                    break;
                default:
                    Console.WriteLine($"Confirm: bad action for state {State}");
                    break;
            }
        }

        public void Clear() {
            switch (State) {
                case ImageHelperState.Saved:
                    tempFile = null;
                    State = ImageHelperState.ClearedAfterSaved;
                    break;
                case ImageHelperState.LoadedAfterEmpty:
                    DeleteFile(tempFile);
                    tempFile = null;
                    State = ImageHelperState.ClearedAfterEmpty;
                    break;
                case ImageHelperState.LoadedAfterSaved:
                    DeleteFile(tempFile);
                    tempFile = null;
                    State = ImageHelperState.ClearedAfterSaved;
                    break;
                default:
                    Console.WriteLine($"Clear: bad action for state {State}");
                    break;
            }
        }

        private void CreateTempFile(string sourceFile) {
            var newExt = Path.GetExtension(sourceFile);
            var guid = Guid.NewGuid();
            tempFile = guid + newExt;
            File.Copy(sourceFile, Path.Combine(basePath, tempFile), overwrite: true);
        }

        private void DeleteFile(string file) {
            if (file == null) {
                return;
            }
            var path = Path.Combine(basePath, file);
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        private void RenameTempFile(string targetFileWithoutExtension) {
            var oldPath = Path.Combine(basePath, tempFile);
            currentFile = targetFileWithoutExtension + Path.GetExtension(tempFile);
            var newPath = Path.Combine(basePath, currentFile);
            if (File.Exists(newPath)) {
                File.Delete(newPath);
            }
            File.Move(oldPath, newPath);
            tempFile = null;
        }
    }
}
