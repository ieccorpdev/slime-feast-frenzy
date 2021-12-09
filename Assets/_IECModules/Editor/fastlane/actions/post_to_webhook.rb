module Fastlane
  module Actions
    class PostToWebhookAction < Action
      def self.run(params)
        require 'faraday'
        require 'json'

        bundleId = CredentialsManager::AppfileConfig.try_fetch_value(:app_identifier)
        appName = Fastlane::Actions::GetInfoPlistValueAction.run(path: "./Info.plist", key: "CFBundleDisplayName")
        versionName = Fastlane::Actions::GetVersionNumberAction.run(target: "Unity-iPhone")
        buildNumber = Fastlane::Actions::GetBuildNumberAction.run({})
        lastGitCommit = Fastlane::Actions::LastGitCommitAction.run({})
        changelogGitCommit = Fastlane::Actions::ChangelogFromGitCommitsAction.run({
          pretty: "%B",
          commits_count: 5, 
          merge_commit_filtering: "include_merges"
        })

        # UI.message bundleId
        # UI.message appName
        # UI.message versionName
        # UI.message buildNumber
        # UI.message lastGitCommit[:author_email]
        # UI.message changelogGitCommit
        
        Faraday.post("https://us-central1-united-data.cloudfunctions.net/testflightNotify?channel=tester-general", JSON.generate({ 
          bundleId: bundleId, 
          appName: appName,
          versionName: versionName,
          buildNumber: buildNumber,
          lastGitCommitAuthor: lastGitCommit[:author_email],
          changelogGitCommit: changelogGitCommit
        }),
        {
          "Content-Type" => "application/json"
        })
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.description
        "Post Build Success Notification to Url"
      end

      def self.authors
        # So no one will ever forget your contribution to fastlane :) You are awesome btw!
        ["thanhpt"]
      end

      def self.is_supported?(platform)
        platform == :ios
      end
    end
  end
end
