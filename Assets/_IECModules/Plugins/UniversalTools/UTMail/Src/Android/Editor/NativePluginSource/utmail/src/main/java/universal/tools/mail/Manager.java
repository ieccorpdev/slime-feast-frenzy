package universal.tools.mail;

import android.app.AlertDialog;
import android.content.ComponentName;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Build;
import android.text.Html;
import android.text.Spanned;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListAdapter;
import android.widget.TextView;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import static android.content.Intent.FLAG_GRANT_READ_URI_PERMISSION;

public class Manager {
    public static void compose(final String annotation, final String to[], final String cc[], final String bcc[], final String subject, final String body, final boolean htmlBody, final String attachments[]) {
        final Context context = UnityPlayer.currentActivity;
        final PackageManager packageManager = context.getPackageManager();

        Intent sendMultipleDummyintent = new Intent(Intent.ACTION_SEND_MULTIPLE);
        sendMultipleDummyintent.setType(htmlBody ? "text/html" : "text/plain");

        Intent emailDummyIntent = new Intent(Intent.ACTION_SENDTO);
        emailDummyIntent.setData(Uri.parse("mailto:dummy@domain.com"));

        final List<ResolveInfo> sendMultipleActivities = packageManager.queryIntentActivities(sendMultipleDummyintent, 0);
        final List<ResolveInfo> emailActivities = intersect(packageManager.queryIntentActivities(emailDummyIntent, 0), sendMultipleActivities);

        if (emailActivities != null && emailActivities.size() > 0) {
            if (emailActivities.size() == 1) {
                composeInternal(context, annotation, to, cc, bcc, subject, body, htmlBody, attachments, emailActivities.get(0));
            } else {
                // Is there a default email app?
                for (ResolveInfo it : emailActivities) {
                    if (it.isDefault) {
                        composeInternal(context, annotation, to, cc, bcc, subject, body, htmlBody, attachments, emailActivities.get(0));
                        return;
                    }
                }

                // No default: let's pick one
                final AppItem[] availableEmailApps = new AppItem[emailActivities.size()];
                for (int i = 0; i < emailActivities.size(); ++i) {
                    ApplicationInfo applicationInfo = emailActivities.get(i).activityInfo.applicationInfo;
                    availableEmailApps[i] = new AppItem(applicationInfo.loadLabel(packageManager).toString(), applicationInfo.loadIcon(packageManager));
                }

                ListAdapter adapter = new ArrayAdapter<AppItem>(context, android.R.layout.select_dialog_item, android.R.id.text1, availableEmailApps) {
                    public View getView(int position, View convertView, ViewGroup parent) {
                        View v = super.getView(position, convertView, parent);
                        TextView tv = (TextView)v.findViewById(android.R.id.text1);

                        tv.setCompoundDrawablesWithIntrinsicBounds(availableEmailApps[position].icon, null, null, null);

                        int padding = (int)(5 * context.getResources().getDisplayMetrics().density + 0.5f);
                        tv.setCompoundDrawablePadding(padding);

                        return v;
                    }
                };

                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.setTitle(annotation);
                builder.setAdapter(adapter, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which)
                    {
                        composeInternal(context, annotation, to, cc, bcc, subject, body, htmlBody, attachments, emailActivities.get(which));
                    }
                });

                builder.create().show();
            }
        } else {
            // May cause non-email applications in the list
            composeInternal(context, annotation, to, cc, bcc, subject, body, htmlBody, attachments, null);
        }
    }

    public static String getTemporaryDirectory() {
        try {
            return UnityPlayer.currentActivity.getExternalCacheDir().getCanonicalPath();
        } catch (IOException e) {
            Log.e("UTMail", e.toString());
            return "";
        }
    }

    private static void composeInternal(final Context context, final String annotation, final String to[], final String cc[], final String bcc[], final String subject, final String body, final boolean htmlBody, final String attachments[], final ResolveInfo resolveInfo) {
        Intent intent = new Intent(Intent.ACTION_SEND_MULTIPLE);

        intent.setType(htmlBody ? "text/html" : "text/plain");
        if (to != null && to.length > 0) {
            intent.putExtra(Intent.EXTRA_EMAIL, to);
        }
        if (cc != null && cc.length > 0) {
            intent.putExtra(Intent.EXTRA_CC, cc);
        }
        if (bcc != null && bcc.length > 0) {
            intent.putExtra(Intent.EXTRA_BCC, bcc);
        }

        if (subject != null && !subject.isEmpty()) {
            intent.putExtra(Intent.EXTRA_SUBJECT, subject);
        }

        if (body != null && !body.isEmpty()) {
            intent.putExtra(Intent.EXTRA_TEXT, htmlBody ? fromHtml(body) : body);
        }

        if (attachments != null && attachments.length > 0) {
            ArrayList<Uri> uris = new ArrayList<>();

            for (String attach : attachments) {
                final Uri uri = FileProvider.getUriForFile(context, context.getApplicationContext().getPackageName() + ".universal.tools.mail.fileprovider", new File(attach));
                uris.add(uri);
            }

            intent.addFlags(FLAG_GRANT_READ_URI_PERMISSION);
            intent.putParcelableArrayListExtra(Intent.EXTRA_STREAM, uris);
        }

        if (resolveInfo != null) {
            intent.setComponent(new ComponentName(resolveInfo.activityInfo.packageName, resolveInfo.activityInfo.name));
            context.startActivity(intent);
        } else {
            context.startActivity(Intent.createChooser(intent, annotation));
        }
    }

    private static List<ResolveInfo> intersect(List<ResolveInfo> a, List<ResolveInfo> b)
    {
        if (a == null || b == null) {
            return null;
        }

        List<ResolveInfo> result = new ArrayList<>(a.size());
        for (ResolveInfo it : a) {
            for (ResolveInfo it2 : b) {
                if (it.activityInfo.packageName.equals(it2.activityInfo.packageName)) {
                    result.add(it);
                    break;
                }
            }
        }

        return result;
    }

    private static Spanned fromHtml(String html) {
        if (android.os.Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            return Html.fromHtml(html, Html.FROM_HTML_MODE_COMPACT);
        } else {
            return Html.fromHtml(html);
        }
    }

    private static class AppItem {
        final String text;
        final Drawable icon;

        AppItem(String text, Drawable icon) {
            this.text = text;
            this.icon = icon;
        }

        @Override
        public String toString() {
            return text;
        }
    }
}
