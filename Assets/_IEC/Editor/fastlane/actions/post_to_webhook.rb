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

        # UI.message bundleId
        # UI.message appName
        # UI.message versionName
        # UI.message buildNumber

        Faraday.post("http://discordbot.game.ieccorp.vn:3004/", JSON.generate({ 
          bundleId: bundleId, 
          appName: appName,
          versionName: versionName,
          buildNumber: buildNumber
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
