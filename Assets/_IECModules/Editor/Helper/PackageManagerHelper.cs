using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace _IECModules
{
    public enum RequestType
    {
        AddRequest,
        EmbedREquest,
        ListRequest,
        PackRequest,
        RemoveRequest,
        SearchRequest
    }
    
    public class PackageManagerHelper : MonoBehaviour
    {
        static Queue<PKGRequest> listRequest = new Queue<PKGRequest>();

        static PKGRequest currentRequest;


        public static void Add(string package, Action<AddRequest> onSuccess = null)
        {
            var pkgRequest = new PKGRequest()
            {
                type = RequestType.AddRequest,
                package = package,
                onAddRequestDone = onSuccess
            };
            QueueAndRun(pkgRequest);
        }
        
        public static void Search(string package, Action<SearchRequest> onSuccess = null)
        {
            var pkgRequest = new PKGRequest()
            {
                type = RequestType.SearchRequest,
                package = package,
                onSearchRequestDone = onSuccess
            };
            QueueAndRun(pkgRequest);

        }

        public static void Remove(string package, Action<RemoveRequest> onSuccess = null, bool skipError = false)
        {
            var pkgRequest = new PKGRequest()
            {
                type = RequestType.RemoveRequest,
                package = package,
                onRemoveRequestDone = onSuccess,
                skipErrorMessage = skipError
            };
            QueueAndRun(pkgRequest);
        }

        static void QueueAndRun(PKGRequest request)
        {
            listRequest.Enqueue(request);
            if (currentRequest == null)
            {
                RunNextRequest();
            }
        }

        static void RunNextRequest()
        {
            currentRequest = listRequest.Peek();
            var processName = "";
            switch (currentRequest.type)
            {
                case RequestType.AddRequest:
                    currentRequest.InitAddRequest();
                    processName = "Installing";
                    break;
                case RequestType.RemoveRequest:
                    currentRequest.InitRemoveRequest();
                    processName = "Removing";
                    break;
                case RequestType.SearchRequest:
                    currentRequest.InitSearchRequest();
                    processName = "Searching";
                    break;
            }
            LogEditorHelper.AddLog($"{processName}: {currentRequest.package}");
            EditorApplication.update += Progress;
        }

        static void Progress()
        {
            var request = currentRequest.request;
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                    switch (currentRequest.type)
                    {
                        case RequestType.AddRequest:
                            {
                                var detailRequest = (AddRequest)request;
                                LogEditorHelper.AddLog("Installed: " + detailRequest.Result.packageId);
                                currentRequest.onAddRequestDone?.Invoke(detailRequest);
                            }
                            break;
                        case RequestType.EmbedREquest:
                            {
                                var detailRequest = (EmbedRequest)request;
                                LogEditorHelper.AddLog("Embeded: " + detailRequest.Result.packageId);
                                currentRequest.onEmbedRequestDone?.Invoke(detailRequest);
                            }
                            break;
                        case RequestType.ListRequest:
                            {
                                var detailRequest = (ListRequest)request;

                                currentRequest.onListRequestDone?.Invoke(detailRequest);
                            }
                            break;
                        case RequestType.PackRequest:
                            {
                                var detailRequest = (PackRequest)request;
                                currentRequest.onPackRequestDone?.Invoke(detailRequest);
                            }
                            break;
                        case RequestType.RemoveRequest:
                            {
                                var detailRequest = (RemoveRequest)request;
                                LogEditorHelper.AddLog("Removed: " + detailRequest.PackageIdOrName);
                                currentRequest.onRemoveRequestDone?.Invoke(detailRequest);
                            }
                            break;
                        case RequestType.SearchRequest:
                            {
                                var detailRequest = (SearchRequest)request;
                                LogEditorHelper.AddLog("Search done");
                                currentRequest.onSearchRequestDone?.Invoke(detailRequest);
                            }
                            break;
                    }
                else if (request.Status >= StatusCode.Failure && !currentRequest.skipErrorMessage)
                    LogEditorHelper.AddLogError(request.Error.message);

                EditorApplication.update -= Progress;

                listRequest.Dequeue();
                if (listRequest.Count > 0)
                {
                    RunNextRequest();
                }
                else
                {
                    LogEditorHelper.AddLog("Process is completed");
                    currentRequest = null;
                }
            }
        }


    }

    public class PKGRequest
    {
        public RequestType type;
        public Request request;
        public string package;
        public bool skipErrorMessage;

        public Action<AddRequest> onAddRequestDone;
        public Action<ListRequest> onListRequestDone;
        public Action<EmbedRequest> onEmbedRequestDone;
        public Action<PackRequest> onPackRequestDone;
        public Action<RemoveRequest> onRemoveRequestDone;
        public Action<SearchRequest> onSearchRequestDone;

        public void InitAddRequest()
        {
            type = RequestType.AddRequest;
            request = Client.Add(package);
        }
        public void InitRemoveRequest()
        {
            type = RequestType.RemoveRequest;
            request = Client.Remove(package);
        }

        public void InitSearchRequest()
        {
            type = RequestType.SearchRequest;
            request = Client.Search(package);
        }
    }
}
