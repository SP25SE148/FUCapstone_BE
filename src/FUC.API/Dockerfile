# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/FUC.API/FUC.API.csproj", "src/FUC.API/"]
COPY ["src/FUC.Common/FUC.Common.csproj", "src/FUC.Common/"]
COPY ["src/FUC.Service/FUC.Service.csproj", "src/FUC.Service/"]
COPY ["src/FUC.Data/FUC.Data.csproj", "src/FUC.Data/"]

RUN dotnet restore "src/FUC.API/FUC.API.csproj"

COPY . .
WORKDIR "/src/src/FUC.API"
RUN dotnet build "FUC.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FUC.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 8081
ENV TOPIC_BUCKET_NAME="fuc-topic-1" \
    TEMPLATE_BUCKET_NAME="fuc-template-1" \
    GROUP_DOCUMENT_BUCKET_NAME="fuc-group-document-1" \
    THESIS_BUCKET_NAME="fuc-thesis-1" \
    EVALUATION_PROJECT_PROGRESS_KEY="Import/ProjectProgress" \
    REVIEWS_CALENDARS_KEY="Import/ReviewsCalendars" \
    DEFENSE_CALENDAR_KEY="Import/DefenseCalendar" \
    EVALUATION_WEEKLY_KEY="Export/EvaluationWeekly" \
    STUDENTS_TEMPLATE_KEY="Import/Students" \
    SUPERVISORS_TEMPLATE_KEY="Import/Supervisors" \
    THESIS_COUNCIL_MEETING_MINUTES_TEMPLATE_KEY="Import/ThesisCouncilMeetingMinutes" \
    TOPIC_REGISTRATION_TEMPLATE_KEY="Import/TopicRegistration" \
    ENABLE_PROFILE="true"
ENTRYPOINT ["dotnet", "FUC.API.dll"]
