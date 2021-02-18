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
            // Add a package to the project
            var addPKGRequest = Client.Add(package);
            var pkgRequest = new PKGRequest();
            pkgRequest.InitAddRequest(addPKGRequest, onSuccess);
            QueueAndRun(pkgRequest);

        }


        public static void Search(string package, Action<SearchRequest> onSuccess = null)
        {
            var searchPKGRequest = Client.Search(package);
            var pkgRequest = new PKGRequest();
            pkgRequest.InitSearchRequest(searchPKGRequest, onSuccess);
            QueueAndRun(pkgRequest);

        }

        public static void Remove(string package, Action<RemoveRequest> onSuccess = null, bool skipError = false)
        {
            var removePKGRequest = Client.Remove(package);
            var pkgRequest = new PKGRequest();
            pkgRequest.InitRemoveRequest(removePKGRequest, onSuccess, skipError);
            QueueAndRun(pkgRequest);
        }

        static void QueueAndRun(PKGRequest request)
        {
            listRequest.Enqueue(request);
            if (currentRequest == null)
            {
                currentRequest = listRequest.Peek();
                EditorApplication.update += Progress;
            }
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
                    currentRequest = listRequest.Peek();
                    EditorApplication.update += Progress;
                }
                else
                {
                    currentRequest = null;
                }
            }
        }


    }

    public class PKGRequest
    {
        public RequestType type;
        public Request request;

        public bool skipErrorMessage;

        public Action<AddRequest> onAddRequestDone;
        public Action<ListRequest> onListRequestDone;
        public Action<EmbedRequest> onEmbedRequestDone;
        public Action<PackRequest> onPackRequestDone;
        public Action<RemoveRequest> onRemoveRequestDone;
        public Action<SearchRequest> onSearchRequestDone;

        public void InitAddRequest(AddRequest addRequest, Action<AddRequest> onSuccess = null)
        {
            type = RequestType.AddRequest;
            request = addRequest;
            onAddRequestDone = onSuccess;
        }

        public void InitListRequest(ListRequest listRequest, Action<ListRequest> onSuccess = null)
        {
            type = RequestType.ListRequest;
            request = listRequest;
            onListRequestDone = onSuccess;
        }

        public void InitEmbedRequest(EmbedRequest embedRequest, Action<EmbedRequest> onSuccess = null)
        {
            type = RequestType.EmbedREquest;
            request = embedRequest;
            onEmbedRequestDone = onSuccess;
        }

        public void InitPackRequest(PackRequest packRequest, Action<PackRequest> onSuccess = null)
        {
            type = RequestType.PackRequest;
            request = packRequest;
            onPackRequestDone = onSuccess;
        }

        public void InitRemoveRequest(RemoveRequest removeRequest, Action<RemoveRequest> onSuccess = null, bool skipError = false)
        {
            type = RequestType.RemoveRequest;
            request = removeRequest;
            onRemoveRequestDone = onSuccess;
            skipErrorMessage = skipError;
        }

        public void InitSearchRequest(SearchRequest searchRequest, Action<SearchRequest> onSuccess = null)
        {
            type = RequestType.SearchRequest;
            request = searchRequest;
            onSearchRequestDone = onSuccess;
        }
    }
}
