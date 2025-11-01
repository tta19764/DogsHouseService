namespace DogsHouseService.WebApi.Logging
{
    /// <summary>
    /// Logging extensions for dog-related events
    /// </summary>
    public static class DogsLog
    {
        private static readonly Action<ILogger, string, Exception?> dogCreated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1000, nameof(DogCreated)),
            "Dog created successfully: {DogName}"
        );

        private static readonly Action<ILogger, string, Exception?> dogDeleted =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1001, nameof(DogDeleted)),
                "Dog deleted successfully: {DogName}"
            );

        private static readonly Action<ILogger, string, Exception?> dogUpdated =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1002, nameof(DogUpdated)),
                "Dog updated successfully: {DogName}"
            );

        private static readonly Action<ILogger, Exception?> retrievingDogs =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(1003, nameof(RetrievingDogs)),
                "Retrieving dogs list from the service."
            );

        private static readonly Action<ILogger, string, Exception?> dogRetrieved =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1004, nameof(DogRetrieved)),
                "Retrieved dog: {DogName}"
            );

        private static readonly Action<ILogger, string, string, Exception?> dogUpdateFailed =
            LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(1005, nameof(DogUpdateFailed)),
                "Failed to update dog '{DogName}'. Reason: {ErrorMessage}"
            );

        private static readonly Action<ILogger, string, string, Exception?> dogRetrievalFailed =
            LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(1006, nameof(DogUpdateFailed)),
                "Failed to retrieve dog '{DogName}'. Reason: {ErrorMessage}"
            );

        private static readonly Action<ILogger, string, string, Exception?> dogDeletionFailed =
            LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(1007, nameof(DogUpdateFailed)),
                "Failed to deleate dog '{DogName}'. Reason: {ErrorMessage}"
            );

        private static readonly Action<ILogger, string, string, Exception?> dogCreationFailed =
            LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(1008, nameof(DogUpdateFailed)),
                "Failed to create dog '{DogName}'. Reason: {ErrorMessage}"
            );

        /// <summary>
        /// Logs the creation of a dog.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        public static void DogCreated(this ILogger logger, string dogName)
            => dogCreated(logger, dogName, null);

        /// <summary>
        /// Logs the deletion of a dog.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        public static void DogDeleted(this ILogger logger, string dogName)
            => dogDeleted(logger, dogName, null);

        /// <summary>
        /// Logs the update of a dog.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        public static void DogUpdated(this ILogger logger, string dogName)
            => dogUpdated(logger, dogName, null);

        /// <summary>
        /// Logs the retrieval of dogs.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void RetrievingDogs(this ILogger logger)
            => retrievingDogs(logger, null);

        /// <summary>
        /// Logs the retrieval of a dog.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        public static void DogRetrieved(this ILogger logger, string dogName)
            => dogRetrieved(logger, dogName, null);

        /// <summary>
        /// Logs a failed dog update.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        /// <param name="errorMessage">The error message.</param>
        public static void DogUpdateFailed(this ILogger logger, string dogName, string errorMessage)
            => dogUpdateFailed(logger, dogName, errorMessage, null);

        /// <summary>
        /// Logs a failed dog retrieval.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        /// <param name="errorMessage">The error message.</param>
        public static void DogRetrievalFailed(this ILogger logger, string dogName, string errorMessage)
            => dogRetrievalFailed(logger, dogName, errorMessage, null);

        /// <summary>
        /// Logs a failed dog deletion.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        /// <param name="errorMessage">The error message.</param>
        public static void DogDeletionFailed(this ILogger logger, string dogName, string errorMessage)
            => dogDeletionFailed(logger, dogName, errorMessage, null);

        /// <summary>
        /// Logs a failed dog creation.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dogName">The dogs name/</param>
        /// <param name="errorMessage">The error message.</param>
        public static void DogCreationFailed(this ILogger logger, string dogName, string errorMessage)
            => dogCreationFailed(logger, dogName, errorMessage, null);
    }
}
