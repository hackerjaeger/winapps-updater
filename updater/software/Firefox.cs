﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/98.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "1244e3c95647ee5ce11491fd25eb3802af952d31655368283e3cde22002fd3cdaa7b809f3f6d2491e9173c0fb5c1dff0157fa8f111ee184a4f5be5dd0f5c34b7" },
                { "af", "ffdcf62177cd05e564831d9faf19ceb9193963ebf22a5609b037410a206e7780b801834fb001d5e42ee8a7fc056afb04379e42430c0f49d8299cb4774740ae8f" },
                { "an", "7773524505eb647d06d7479a58431bd61dcb2f3f553082789306ac50854d5a221857d0e07c974dca7714abae38901605f98f5a2becaae24df33f09a00d488171" },
                { "ar", "150e1030d9edd741c90b485d1f80f2c62a430da12f0fcd20666bc48d62ed459a30d9892ba514a3edbde7cf2e6a0874fff3857026ee426f0b10296e5b05d795ad" },
                { "ast", "816e61efd5d10fb32029f01b823b47d088b359d54910da8bdd74e5323d3a2cb9ca1ad2ec551ce5df60b8b258ba89409a644506bd87f3543ed88107950cf20dff" },
                { "az", "02958945680b5f1f51158ee493df0f81c5dbdab7fa52e3f17ec6b9751c31d391eab485b4f6f12ab25e10a8cc2fa983f896d30b6f00dc0f7a23c0e24ca31cc1ef" },
                { "be", "ce5c14e33c7c5a5aa79ebe2d529204c9691eb4c914b9370471cbc0944a7d07c19da7815e8c3c241a6818b8b8ef5ad7752e3ba8fdb9d556536e4916be242d517c" },
                { "bg", "b4edefd766ac1c12cc9380c167061386f5ef13859c18e8f35efe38c69657919b6dccc44831923b5a697c3b2fe128360f25bf359c626ee2df749db0a558bcdb05" },
                { "bn", "da9b9320f8733311d0da1708a42f53b90bf4f94d296c23934c2af40d4f837f37c73029555ab8077bea84bcf14c9dc56459c295e563c6a779d6581ed3694dcca2" },
                { "br", "4bfb4345b1c6a7d2e58bf7c3acf6b523ec03079fab3ae04753df12d94f037287b1056392ae2650830ac0d086068d911f93f8905c9e5494efae0c0c0cc4397c27" },
                { "bs", "d57ec723a1fc9152017ad806db12768b074cf3b9d8f36b11afd86c558a139d4c9ae721609a90f574bf34b30cc78b1eca9555c54a192d56c40c30055ea5c81791" },
                { "ca", "562aeaf214526260154df91428d16d6eb7f17fadd7cc2b0cb5e3f6493b559d88c3017a0a353e804fb226274449fae3ef8a230262b162b64b0fc9346e009965e5" },
                { "cak", "ad702df0836f90921fc420ea8555e475a934227058d02d22ead0743ef0dde25cb85441476e1f4196da174a8eb17b2724310ace6c31b3dddcc11a1316b7677483" },
                { "cs", "3658e6fd4bae4742e410022b7ffcdae3bd37a2b01f06d006849e9b34bd144187033beaec54f902c5eb9c82287d3b4a6895af87ca8c3f37ad93aec5b98b38711e" },
                { "cy", "2396fbfd05cf5403fb877aeeefc9291ab6b7a3468abcc5d21d00fa3fd10a47ffe24346b6e54850acaf73a175b48ba83e42a0ff6881058249cedb5a3ca3e4dcf5" },
                { "da", "2c154cc823e9bd6a12dfd0f00a66c7bf427f822a243a66b8739ff235b3302408d5d565d3d80c4a9f9fa92cea3ad2a99423d03f7659308a01a6379a35fc22ce0b" },
                { "de", "19aed79a678b649584df2eb76a939590a7a07266d9e56bb97cc09939daff9053b880c747e71bbf24e2a36b7917c0a70c9ae78bcc999907a50a52f3a90cd8feb4" },
                { "dsb", "0c9e42d42e7849a8075c8796932df4505a5305f62082e35d8ada7ee725cb2f86fa0d3f8cb150269b52949634603cdc6a257f6fbc65b4cd9d3b75aa1406dd7092" },
                { "el", "9262b0a0c8222d14927dd96c445a987d63997b34bc28e49f7c7dfd8842a5a0cd46c3c4fd01acccf316cc64403d4dd98b973f3e2bd46e93c50ae10c4413321cd6" },
                { "en-CA", "f1a71bb65d378b0dd849f6fd8e8d2f01a884cd2ab9cad9e02c6167858f5ba4751149ca1f4e33cd32b894e22c88b3db78f0481699069c02677d32d048e8dab33b" },
                { "en-GB", "9f73ae2461ec198ad0904340db04b4cd5d959557663c5ec4037e3d84b56d9d3b33220facec2409ea94414e06b85310d05cb8130f10a8ad9af84dce2482611288" },
                { "en-US", "5f9a18786422da3bc799ded7fdacebef9b4f409c3fe72d41ee9b11adb7e007f3ce7171a8b751adc68953447d5aef144031215511f43a0848974ba103722f3542" },
                { "eo", "f59cac4661829d18ee33a0ecacbad66ef679590abd76aa56a88b9cd46f4f16851f8e90b1171f9363f429e5e764ab041055a378b8d9d8cac2254b0f5010a27222" },
                { "es-AR", "03f5985451570c7d0695f86c73b17549704bd43393e76a9df2a51395238f54aa9cd73b8ee520a94fbddbd53091a9ffa00cf9d2a56a4d5a89a72c00bc8d9261dd" },
                { "es-CL", "13b91feefe8deab7f8ab9a30765e41bf9b3cba531f410978b2e559ac9975d203121bd97cc072b2fabb621e7fef210901a1b30e90226da141aa7d3faa276af3cf" },
                { "es-ES", "1ebab7b15d5ed491e6157f650eb522ba0ed5e61f0a1e7d5f742d6a198388c30df53c57b204ce4d3dda2bc659595d6553d60e16c613aeb941c743ce390842e25b" },
                { "es-MX", "bdf04391ce75070fb8e6114aafcae2f6bed74ccca360fc5b2ce198820a5753d311e8d63f4c3e8e7e8d44a2c746b613f6eb642b6187ff2bdb03a76bf8cccc5a99" },
                { "et", "ddaa018aa5ffa8b711e1ec3c9e34404e86aa6bad80faafb0032f2c3a49240c8110912b617ba0f506b557d404592e5d056152616b6bc5c6082175428e349a29b0" },
                { "eu", "142e8b1e5029faeec9c87514b6b4a7a59e04da17da81cc0d646918d3bda2e4baca7b1e2dcd49fd8551c667d2869a045711e779ea11137b521698ce7f02a0f1a2" },
                { "fa", "527884ef33d5590eebae72e37c8b3adfc166700224b6bae0130e574fed1bde2a9a359f685f2c584c7fcd60b82864273d78e12827d72cd16b1ce8f81a23e91544" },
                { "ff", "8f2689bdc6145f547ae2a3a303b2c934de47d121947ce4dcc3651fb165ffa4ace9cc5245f2a4d5577e4c3885cf6dfd87a00d7519e966fb165d9ea452b0fd22d4" },
                { "fi", "a30421deb092e9f2369a5bd72b57c24affa051f92187c3dde28c539fa0c86e0a8cdce5bc4369225eff1d45d3978ad646a979d33aafa0e55ca38802fda20e8cc3" },
                { "fr", "f02ca9ccfc881f60ca1e1a34a9da136f945577cd7eb13effd8d837a5d54e44a5b404a31020d98d4b760dfdafef52e47159c461650115c5767dc710f082326de3" },
                { "fy-NL", "ed583765111f583d9a808cd105a1ecd23bdb192f546c3f2c3a98e3962a81b3106d266719a8957495b4279042fe2fa285a23c01423cabb5984c70384724022da3" },
                { "ga-IE", "26643cc9d32eadc19385c3d7c4b949ef4e0b0fea69e9272e2907b3572be408aeb16cf613c961d727bec81bc6182cab87d5540c128dff8ff11abc36ce06c5e6bc" },
                { "gd", "3a78655a8ae446a151431b7b89c9e1d4c57c1f3ac7f5410fedfdf746f8e9750cc73a43571694dbc413fcc0e99c87bb0203c5e4c0bd4b40dcd9a3340810d5a060" },
                { "gl", "b9cf14b3505bd74c9ca4ebd5580085f3b7e49db7bc6e5a231ee0057cf124df3aa5944893e7a88542eac9ea90d5ace8257c89b823a36d34c9b3797a0816b56b42" },
                { "gn", "9a929f21a87f6097a4cc48f6d457382602dcb5d94a974cd96e687d903b9068cc9fce65787ecdc1ba86478f31f0e23b1c7699b171f0c033085f0c636bf9138ec4" },
                { "gu-IN", "ad9777ea3faab863a0611271e7b20c5be8e50df3683e7398d5116f4c007783fc80dc44fe82ddc6f970a753b98809ff79a4b73edfb9dd8a91d9b8966eb4d76710" },
                { "he", "182300c1333a36ae33447a8c718fe2a5c9bb5192f6102f780e8107dc30c311e226febd31d2b0c6434e044dc7eee7c1e5dd1cc0ab155d5559975268e0f44e3cb0" },
                { "hi-IN", "1da6f5d62b768bf05e86362e808f60638a08c3754bc8996da098120fb84c66ea013d2d583228e07b4ae73fffe88ef84f6f1916a5bf0f2211afcad15d7090df61" },
                { "hr", "c65ffd07d89a9980e56d4b2a18a106c0f289f136142050518025397478521f3b80f1782c9c7f02c975bbd3ee99e6034d62c5692c8c653994be318c2c5e34ce5c" },
                { "hsb", "cc02e1fadaf44d7ba29f14758c5cb83bae8687645645ec3a6018dbf46634169ac2f06dffd8d486275a20a0f98cbe76f5b4ea3b031f24f6c9b6b552ee7cece8aa" },
                { "hu", "1b38f8fff2d50459311aff2a1598d0a5b4fdc38792df6ba5d24d3f3414e91bd1776132b5abf618f67ce144a8c05ee7b67a6373c8096afe33b5387c09373da743" },
                { "hy-AM", "8627f197a224aebdd52de0b7ea1f54ec24f939919aecc3a2631535103f6fb611a847317f2153f37fb369119202de3eb627a12a178eea1c4fc53d7f4ce38eb1d1" },
                { "ia", "7fa27536a81553143b8b061f22d25a8071285e4b8b38add19c5f19800b6ff56403905941ccb8c91efdde39f1d4cb38a2bde56ab93e7fb5dcbce66bbacc0f01b6" },
                { "id", "23ed5511992d60c0b21725b5b303d979b9a19ea126424ffb4bfe5806897e8c3a74eaa6fe36807f303bf41cf8062c7debef6cb1428d625799b44fa752f68f8786" },
                { "is", "ef3922a28e2fbe37dfbd4eceb90f1e0d000965e290b778c28bdc5ef4b9db0c0b5cc5080b9f402573bfde306a945ee0c9066d212a2ecb14b97c1775bcd4f2ace6" },
                { "it", "d7d7ca4cd37227dae1d228c5da11487ae14ee2a3bc215ebe8504f0e6ccb4317649cf1723aff689571dde6eb21325611f44133b5049aab53d9f673c05efe44eae" },
                { "ja", "e5842d2f46003a6f869230b69574d1187055d108962e2d743571c3de2d07c7461d66ea3d85b08dd692cbd70f9844a2a118e50cb4955de3bd801440c3e6f2e793" },
                { "ka", "9f3b8120a9e3001df3121da3b8e49aeb620a0e526a583b6a16507d8a0cf88035d45d661fb1f34edd69758ba492fdf40ed4d9000d69e0c6e27161e810a7b727c2" },
                { "kab", "6d65be2faba667923b8324a9dc23460228c339d9a09c276d3931ad49bc9cb6bcbade005facdc98d9f05c42aa8de42b3c3ce3ed3117383a67f1bae2c21d4333d2" },
                { "kk", "cf792c8706c8ff741f81b3c9fbe0ced72a22455808e51d107dd552b8c7e55d756e44529a2efb40a3b928ff667acced2a472a45a0aeefc04e64fc315f3ec15d8e" },
                { "km", "358b898d78e3046d09bca6affdc5302fdca5244a430293a08612cb02c1935323733673cbe8cfa7f93ff7935842da914e80c32bb0488b14b3be971f1678eaa899" },
                { "kn", "9c1ac617b7de27da5883c9e901c4c5400b1377f1ba5e02bb06034439fd71572c5cac99efa78a7cc09305b08d879f6494e4c984d488e787070bc635b3d8dfa86d" },
                { "ko", "55a649dfac98c0a4d61b2fbca84f68b685a5a603cea3f4c96aaef608f09a21a5c672fc85177ef312ad70529a32d59da687aa443222842fb26522f5c29343f6a5" },
                { "lij", "7a3426ab0f62c8606a678958b52575d2e3865bad011ed2f670331e53ae2a84b7052d0b8f9a421b96f8d6827607a4e9b53412cc0ca4304c4be9c1605e99f74cea" },
                { "lt", "705860632cd989b3e43c72a40871671c271b8729a2a4b8e0127ab6c7759912c1f630eb07689559bcffdb6bc981a8ad4b757542b47055d3d53f858cc8dcd17e0a" },
                { "lv", "31ead397b47b406a36dc0de878e6ff421d2046eacf896444d4224f0b89822dc332c78514c7660c0535614534b6e1bc73f22d2d4cb7e8e708fd52c1ed6e7fa097" },
                { "mk", "982f310e2d71cea041281270e5d363a0b7a115ae6ee1e9c4fed3606100ca47e4d0b76eeed63fa4bdee1bf44773dd272ccfdf64ca65e44ee626e0fa20742abf36" },
                { "mr", "8a187b14a4f02b5d2fad834e246560a3c2e9041298bd02b62adb79001b23ae3a915c56138c6dcb61eacc0d80eca53ba11703b4deb4b3ff495d5ec5b1fa1e2736" },
                { "ms", "172b3ca5e56c7b4aaa6f791c1cc2c1c621b250081d81daf28f54152aa52746c09096209c974522e6f5998e85416efd4d26962a5bccbbedff3e4dcff09bb45d03" },
                { "my", "2cc7515d222ec4955697f279de7099a1b745f4658bba84356ab47e421509c808af9ead348be1d3f090607d396016a06e23add901f765c9600cd56350e7d96c84" },
                { "nb-NO", "7d46098b310024d107f3e9d8158f2c47e574ebd6e0aed9525959297da80787c5632270708c7b9fa35f19531505a9efda38a240acd59f83901235d684a7d11bd7" },
                { "ne-NP", "f8dea5f6bf6622739e13e59acc8b39bcfca53bb4b16bb9d62dddf1a3a0cb962e70efea17afc7633d98964295fa79d35f07ea9d6193853db2b5024f3dba8d2d58" },
                { "nl", "d4ffdf8679d52cc592dbd158f2d904a3ceaae2a7e693215571b5b0afbd0f4379864477a929d59bd3477dadfe40050e4e80c85110ef8c8268883a813fdf40a4f6" },
                { "nn-NO", "729066b28ce6f4f2f0a17381572ee5c1544de5b883aa973a9eae2d430ba23f294edfc2989cdf7baa70e019c6549a61de4440c370d77c0cd3d948e2c2bcf3852a" },
                { "oc", "dcb595cea5e1551b24ee13c12d53ade446e31cd821fbefda96ba3e9ae9474127ec6b2b156c585a40c9c86384816e06720926424323098963d676fdf6d90b6081" },
                { "pa-IN", "dcfde4ab517befe342dbb249309068aea5faaa3a573c92ea4e468bf6479f5c22dfddc4f464503d588849eef2a57851310e5a0a261358fc32ee9fb97ce313865b" },
                { "pl", "3e2a1e2489b72990d2f36296a016a93629b4761971fffe037cd21d62e6dadd0077746aa608276836d2611b18642798ef5620e58fb66fc616f423ff297afbb984" },
                { "pt-BR", "8877bc805b1cc8e4cc18d2e3ecf991b91fd24a3a564f996d8515c8b46349ed5645c5cfe355b859bb5247ed302430e5f5a2e86c6953dd97e31e3b8119b9be9bf3" },
                { "pt-PT", "cdf1e10b592d91e08c4aa391e15bef02610d3edf42392fea72507874176fb6ea8efe2cc61021dd3037abc10b93ef14094a0f256cf5bb2d33af71d95ddc33bd73" },
                { "rm", "77963ddd262a5f3ede5aa56a34f6f3967c58625969951637431c1b671c255042680edb2748163cf95c95dcee165248e363134de67241fedbe61f701be8633a36" },
                { "ro", "0a466e80d96c9686b433f6bd3e2310ea094acda707c3c723a93af0ec6e9f4788213d400255c2b23e82bd52b5d1a03f3c9cda0af23ae858e4c7b0c1c5c67a5f4f" },
                { "ru", "b7baf17a481ab7e69e00f138851cdf2bb4c6e67daf0f5bcc55238190213a05b17a7ceefa8949b75f011f2336355580b3df67efa955c077aeb22903d48e89e71e" },
                { "sco", "217a3a19f959f296d11c0069ea6cca5bb315e22024730a7bc980f1a3e6743c5b64dbddbe350e5a074eb2f6b7c496fb1bfea49016ab12cfa2dcec21c52a4d436c" },
                { "si", "b9c92bb1cccfc3ee0c5990cd19f95ed5586b0bea7359612f792522f44c0c0b22d80ec53679d188ce3b492b0c28fc42e563f01fe2b96639b8b0ac89a807b8c750" },
                { "sk", "1f8906d1468f6c380d3d469fbb2e6444b4b72d6d39158341b6e51310181be4689b19d6bf41d32d61982c2d8199de553669cc35d4d866c8a72bf245872dc89e21" },
                { "sl", "3c4b456e0bf98c624555079af71fef3a2fe148dca37afc24cea997d26000a2b24769be4cffff33b84f72d989bef2d989715621319bfcbd5205e8cdcad17c4556" },
                { "son", "adbf7df7ae79660bca1ea58a439c0ebafa1105ebb5819adfe4daeb994cac254dd0548909c4a06b57e76c52adcba440d753fdc7635725361a5d75da5d01f551b6" },
                { "sq", "d9e5a34f0b797519e5cb3dca16bee77abcaaf1ce85ab03b59249ae8d7b3f1d1186e447890b6eabd8edde9f2ae3c291a34585ac8ab0ded087fcd500de92f149a6" },
                { "sr", "f93ab96bd1928f70be12de006d52d935a484f6a5bc8ce42da109b6edc7a278464f1a93d894e6cd4e6a67726e5b423844c44be98667082ab85a203acf84f3caf3" },
                { "sv-SE", "91bf8d3bb1b42833381f48a2fef899c62554ea90d2cb9a30aacd85c38ae28a9d4c4bf7a2ed76c320835097ae0c9f535928e202f3fe236c08025867294c584b9f" },
                { "szl", "6f22bfe0f7eea27fb1153813c3b28f0e5b6b2a375fc69a417f97cb8dda7e53d37f61d670e37901495b626a8ca7334422b7f30df04ca87fb0bb0dc441e5ac3dcc" },
                { "ta", "8aacf2c99a569dc598da22a9e82f1c26cc54448ad3d3f7444c016ae0f40860b9a25b3dc877573904dcbc8292909c5650aee6ccf993cbdf93663474c86ef36629" },
                { "te", "25f58521cea43270780e185374688e753c984c6a6200a01bd9534b23c9acded86a9053dc796c50fa0fd7c30ff7bdd883e703baece099c4bbb59447f782d65363" },
                { "th", "ad1f3e77fc95013402fdbb8f2e85ed37b775d8ecac6e00edd083e5f741f4ba6e8dab585d273d3466772167fed5bb571028b8e2b53fa159447025bf5d1425ff00" },
                { "tl", "901bc29519190162b05e68e6d0e84a86d6921306fe97eff982294868315296467c4716bac73a8c3abaa907e3d1806c19964f25ebf0fedfa1d326f18cef5656d8" },
                { "tr", "9889769787d61e981d36861c191f711bf82f9760b4893b8abec4aaa78e19596f39357c74e933257a4d39d907ca79b68ddbc43da2f9d51823bf2b29d79094903c" },
                { "trs", "621971795d79bdc54c7ae5489bc4b67dae6c5c650ab6b6814f28aa860ad4dc548ac8c57368b4889281e14b4c1536a4c2dfce8efac6e0e0a7b832966d76f16f6e" },
                { "uk", "a3d504e5fac2fa7773d4a3b7afd1f371f3f0da6891009689999ea482467351c73e304424c95e5d942a543d7b5c39bcc0c752c50664a08508d7029a4f4cb41c03" },
                { "ur", "4c406c753cb70b57273430e086e7bf8a8934f2e4646e4fefac51f90809e389a447873c783ba6bee6eaa1ed274e727c7ee09646bdd41120de6a112e4eb00a3c97" },
                { "uz", "f2efeb3f47aaecee47b7909459ff0a2a662b22d547bab15789276410f0f378a5ff0c7fb1802b61e0769044bb3fd93bc6f897a15f8599cc3d95136adbb0bdeea2" },
                { "vi", "4e11ac0c904cd019b8a619f593fb8421c31293d9f2cc2075e1384e8ac3d8b83e280bc100c9abef457ccdc90cf1fe31ae073b1768e966a573b22734538aa283e4" },
                { "xh", "af00de700e9b17d53351686cbc7d617a270aa5c1422264767c2ca359756e5331d78a2b972a8aa56f750d7a0a4cbc45ee38ae2049c144b103a96f550762859a82" },
                { "zh-CN", "0e0c17d4adfa12bd24f74f0750f6dc336468d7252f4b5fce0c1537987b2517148728194421fdd32c549dadb3f80a3af26ae539b22d4bb53893b5a57a4100c58a" },
                { "zh-TW", "6106a3cb4d066719b2613e26b550b6216a9587ca2354ef7f6e237b9e03010f0ec7e2b36079c574143ae2fd355a0505bac4e0123758d1ed4d244065e1b08b9723" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/98.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "7a02486ed1aa057d2834c5971dc25878f923770ca876c4a8ec01abeba68aa5c758c977bb89cd2b37d3b2301ad5052f0cd9d5387dc7999a6fcb51e669da16869c" },
                { "af", "03b0cc266c77d92e8029f3ac48b86babc4cfbcfeb0dd34f0fc6441ef051ab73d9147501c0f38a3dacc04e8cfce5a3170a929acc7838092204ed87bd6b29c70d5" },
                { "an", "2ff6fcd0dc388a5771a4eec2603901f2aa2e50be91299bcad5bbf3b7dc1b8d9499f5ef353391a2bc10d751800c7db13ae4a0d58f160ee7384b73fcdefac8c767" },
                { "ar", "9a114a6dcd0a84ad1e1352888ac8c58b22372a3b3b44a3844a33723003cadd14823279bdf52cfe004dfc9a8d30b28bb3560bc634e7964f136cf7dfdfe7dad7f5" },
                { "ast", "5f3508c82dcfef2e310247aa26de5d06d37ffcf5dbdc9e85b3973951f058e84202f1eaf73ebe636885e95f2632e34adcaebcebcb5a2f2efd1d9b7a7f182dbbb0" },
                { "az", "7aaa0714d331a6d329c899801212a6a19f9f728346674d60e589b002f9ffd352018ec64982d7c40bb4412d1da8e47212d26ccbb250395ba7a2c10b82a05c05b9" },
                { "be", "e6d7dc7116ff1fb0b7d7bedac535d1c426f779ea8ca9f290c4cc584f33db15418c9ce6f587edd6b7ad860fdbb34f0f029d1c215b4998c76f261dd421fee9f017" },
                { "bg", "81bbb147b31755699057756cd77ce5a9207577ca71120fcd37d8fdc15d45e7eda691a30d3e24160ad32e798d240a12b0dc314bf1c0f0ef15134381a21af0aaa6" },
                { "bn", "0b9cf1335b4980db9764d34660da6a6028aaa8cff4bf58fe9847830c12d013f10598d5a2176f5aff512b5dbe1346de4e39a045bdd4fd1bee60dbff4abaef6a0a" },
                { "br", "e2963e7f63db346a51fa43f6ba4af7cd2202097bc199177be93fbf844577c6a4ce39b0bd079d90948d2eca006d73fdac871c7b422790d6e86f9210cbe37b6e91" },
                { "bs", "4627a9f520df0b72f3457c3631de698b6dc37c91a15252f8f19cd050e62e9af5314bfde8bebe04ef558ed8b4b654fa4b99cbcf29c3e3f9c990f2520aa4a987b5" },
                { "ca", "eee60d20f5509a78431fa9ff4647f621b3f4327fe11d06d5279efbb593ab0499a4132a8dbc3301af53814a244c028c749b6ed2184357b78b480c0f215de6b52c" },
                { "cak", "48d210c5c219b4437c0bccc6973f5593618986d2ac83a277d7d49988842e835d531754f92cd56674642d4250935c4396338ed2219c651315fa18aa5bbc8156f8" },
                { "cs", "b7dcb04c64e796de6f03c150feeb12eb7c536ecc8d66171c8b5a18e5283d1165931fc07a75a5c8283bbeb799f0ff0ec5373ccdc3f695a706ac39e47736b8ff39" },
                { "cy", "fc6c7bcd39961bb1645ab6f84b300a5110a1903c857352ed40972f05345149ac1334004ab221321cc7982fc8319c473de73d3a856f85fce0809afb0d69e6aca3" },
                { "da", "f14407d4c4981dd1bbb6c03b6dc08a0ae3502d7763415f28db2938d258fa73835ab8800f2d446bdadced965e612755355e5fcbc818ee844795ffc73fbe59457c" },
                { "de", "8de7ca8ebd7f8f4a6ddd9bf516cbf95d4b12be20d2aa4a0891a7eb8c07bee91f0fd3c0c7cb1982e6f7e4331752f45445ff089029ad4b725318d23abe8253967e" },
                { "dsb", "d585d0f1a680e91cfddf807845e02272a08ed0d52f7908cb3010cea145f8b3b58f83638bc2cce7cf9920bf869b04e364eb59d4d59d876556f16252cb538fce96" },
                { "el", "7e4132fc1f055b29478b2c4853820527ba89a511257ec5d1387a8378fe6ddac9be572b37df65751c835195ed7b7787cf5badc0ab3e5aadf518f2da23c4dfa6b4" },
                { "en-CA", "f08a6398f0bdbceae4a9967216a41274faa705d1451b153c501baa49a845bc0e1b488aa67049195c3ee9cb6de44b7845a1514b36da9ec469dd479d2095898c39" },
                { "en-GB", "fc214f5ea26851d952ec1393af2f5df9439c50be61f71164a932674b5edfe86ffcaea232d2492fe6f601fcdd36c6df09a5ee1d561db0fc359afc893c4003fb1e" },
                { "en-US", "7a0fecf89b4576364584b3564b7e4240b9272a30382298e26d2883339df7848b6ec3e59d8c442bcd66cb7cd37057c38e593796780460339d5e94e7f8d539824c" },
                { "eo", "707809cc83f0745b870a535ade3d55d79510e284ae0cb77bc47830f909e3928ed500c65f1363188ef6edfb76068496cade90d5d6424f72422c98f24fa133c3e4" },
                { "es-AR", "b6b35bd19ac3f9778543cc68ebb3f64c2cfc89ab2c0aa0b53d6e339d5a91ec6a5120a765054c24edcea68031a65b1e89f3ab9dcab122c74a4451d244c50c41f5" },
                { "es-CL", "9c7f522dabb28b67b216df5d7df307eb49c9bca4ac71b0f8a9f82fc1bef9efda119c98bca8e81abe46e06675de8d9528cefcb34be7658f28237c27ece3b1bf92" },
                { "es-ES", "6748332bc06cf33157d917408b2910bea5e7ab37636fc20a7c02b34b727deda57e38b854aa64bb03dfcc80ae2529f02bf7aaed7c2b675573410d29dde1d3a245" },
                { "es-MX", "7c0186a27d04aa383de79784fb4aaf83de0c953e6b9a94c0bc794da801c72e64544fb1d0d3cbfd2eab8c5526594132eda48999fe35cc0cba700e2f45e328b09e" },
                { "et", "19a93e8e3911429589b4196b0ace97fc751eeb121a6f8554aa565749f99e098d4ac0dfd300e8565d3e7b84b0dc027fa07db106dadca58d9f37d8ea3d678de1d3" },
                { "eu", "9c8aa288366a8230bae0fd829ff1323b8d9970bda808bc43ae999f8f164f79fb3c5f99c51cb41d50c89ffd6ee0702fe3e8c89a6aab9721afdbd05d9410843f0c" },
                { "fa", "7e1a5180474b1d6393bb81db26a5aafee9d6299f4952b09274fda41b28f0bfcc83499684b09669708e20305f0f6650d3d5592ce58677b71ad754315c8dc5ed65" },
                { "ff", "42dc8c1c8fb771496a9688556589c8d6148afd7a1806fff0d9cb8de81ed96f6c8cb5a9f645f047171e3e83c6ec3a2dab6966aca9d4d0650fc94657d97e93fbfb" },
                { "fi", "bf185c4a9d112ab9a382340f1810b7b26a14b7b21cc8bf48dfdddad5c707e09a97c5d0832a17f1eb09f508bf83fb61b9b9ec7ed721128857877caa74b5bf6486" },
                { "fr", "a82e2e4a6b0f0bb97d667885a37542b8586a09ee692ce3c78385c8a3cfb628affe3e3952e7316753643adda71b4407b48bb7b224a96d6c831f7cb864ba3559eb" },
                { "fy-NL", "e8a20f14699305a4bb8e17e87fd00953fd2271d4b73cdd5f80b066afcded4774f5b4e27df1d378a9a02f8560d375b0a82dd629b73bc8e84fd1ebdb50741d264a" },
                { "ga-IE", "83d1ab7bc3971daf31ba5b2d3a7e2159c0835891ec7ebe0594eb9d23eb25507af45083b281d71294364c331414994d411cabcc587e4adab214433acb7d53b5a4" },
                { "gd", "153e7b6a333be47753720f769276f52f2a7dc2047179e7adbde260780395b7ee656250f666176deea94ffbd645cadf96a351f5c77f39c58dd7d60cf6782baf7c" },
                { "gl", "89fd65ca3fa2cd1cd184e9d0217d16d0f0b475de5907e5f5cbb2409d92c9e407fa98e966a2937c6130d8ccdf32f020807726cea91c119ebe555e5e44bd23a5f4" },
                { "gn", "4cb2bf0428c0e18df39925738a89a80151b47c24e9dc7eb8e6a62b364e32aa3240059efda252adf83419077851add3af5233e577b9b95fef309c47d7625a212e" },
                { "gu-IN", "1ac3ecdc20c47757714845783af5ce0e58799198abb75541dc743448630ac938479394e2dd3f071e4d4d0397bda79fd5d9f153d75f2b305e46fd5526dce6eb0d" },
                { "he", "fd0590a34b9ec7a51ce937685372518465eaa5305436ea9b6f7dd5acd32311b6b7af1db64f901b36bb0bae5462c6deaddad5eecbf1b2c943f35bb04d7fc3030d" },
                { "hi-IN", "cf5e3bb0dc91fbfc139615cd013b2409c143d3c4f7f0443acb29989f25c00de0ac00c7fbd170d9de04c38dc5680cf52c9c58f61e5fd4ba7fd772592aed25120b" },
                { "hr", "78f67e748d9d42d3da84462eaa3d5e0be4f18c1ec92335d3b2a4e9222fcf275fbd51f1eb34ec550cf02bf99c806bb0b3839d90ed73ec4efc9dd630b9a0ca04fb" },
                { "hsb", "597b620f4850ec928d828b036489e36a8fd2d2e0ba5f3bc6ac622bd2de866697c19269f5ba6f53fd64087f2d5a5f396a2982fa170dd651fcd05abc18c2354df9" },
                { "hu", "4b386778685e65e2ef1a0e3b9bed55bf6284647b64487bed677b045a1d780c75f1b5d84e50768fe6c5153e7b19815ed60358cb23240ee8e705f7ae9b2195428f" },
                { "hy-AM", "af86a59d9bace16537c58dbeda23eef84801514a17e339975ed21122fba4b62b7798ba6b2fd8bc8af67e775016983712ab3cfa6905c6810a4bee86a1c595688d" },
                { "ia", "1f2c3b574c6bd51279e4b6090f52393492e2fa0f17d734b83e738c1ac4b7e812b5a32654b32c635d32e5fb0f1d633731d8e39aaa956d142473061a497c265e9e" },
                { "id", "ca1f720bacf560970b8acd6686de678528750332d05b5b4f5d99e6c6ca0a9544053ee8455dd76a72f96c24c0f947409e47bb3e16352c90587a49ebdf21d79377" },
                { "is", "c144d9ea0466856a27a3fc4dd922c6d82bcf850ab69b08e4f40d4ee6791239f9b45ed5156955907cb2549a43f3da9b80ebe42e626972978a313fa1bc9e337139" },
                { "it", "20695ba6f6b2bb5231c2175c22a69c3949f2cb6226c16ac2a65baaf4e8ab1b5f3cababb8a860f00553d4e636af00d4f4697ad65602a4fb395258a5995ced6adf" },
                { "ja", "e31662453c9d064c0a2e5e144c7605012357bf4d3ca45303dba4469d64b26bcd09f8e20e3df275d938f6ff8529ccdb50ce54f9404a58c5d2e70d96916e53f57d" },
                { "ka", "4cce7462b80636edafefc27cac3ecc4da42936c7a571125341c2581d7074cc1e98f60f7c8f6a6f869065e32eff86e30d842f04ce4e71fe68f4ec46d3b7537168" },
                { "kab", "da45a09934f3e3fa6658d2d3d68a0f6b6e54b013059ba788216c331dd341a79fb1e706c3fdc01dc84d4969cd674dbc7d18924c1fb0aaa9b3feb5962e1dc05a55" },
                { "kk", "3fdf7e25a21c37fc590235e2de7cc0056e1366eed7720fc52ac883cbe2ce9437cb88a68a6bf90f40927447af86b6f5a2152b9bcda3658634bb8fd0d73765afca" },
                { "km", "879ff3c19113a833c61871937d036445d18cc90c58a2428d30c2979d1d810f0850867071b18eff949416f081f2164061fa79ccdbc65b72d0e6707d807e056fe3" },
                { "kn", "036262e9b537a05d57f85e91f0b8cf3bc225f7a3b0d00c73f2675e15ee1faa54d34c816cc26719d854220b7d951de900a9fad66ff4dc70c9c5bde5709db52390" },
                { "ko", "6e821cbfe4f8c536533939fd0d8432ec90f6ebb4e04a849f9d7bfb3f9128521828076720c7d59714be070e71a6e2d91ee64d6726fc6ca1fd6f9cffc7c3b2c8dc" },
                { "lij", "0f7fe3739e671ba094be2a88f8b0acde451ef7bba3144036230ce81e6403ba6776916677c3fd36217a1e37a88e9a49663e27c5361790172049ee9e41b88a0594" },
                { "lt", "b485dfb49eff0cfa5bdb15becc1483961557b12dd30d066b367861cef8f9b2517f229120b5de84719b766b941dcff385ab0b82cd13bc19ad988a706b60cc64b6" },
                { "lv", "5837fdede2e4598e7e4167e4673dc3a5a479f3f48d43a3b3f1886ef5d5692b6db31e6df591687d2ee00288f904d42020c169283acff326677b0ebb692c8ce4d5" },
                { "mk", "72fd199e47a1544d745c6a86c8ceb19f327716d034268fbef70a61c3496d5b05a9ff1372ba8cdcb7cbb7e5d58b40f61e02efb971198589abdbf14a11b158daa1" },
                { "mr", "c6be81ca45f1591610ae5d2070427892bf51993a130ff034e52ae903fbd160100e151553c16492c163a87c01f94b168ad5d7ee2d17a715109444f770ceb8934b" },
                { "ms", "c741f3705af247c8fc3eb8094c15332b69f34de21f20e1c55941792627da6ce5bdbecade1d71c39e68b182b9da789cdda67a775bdba98f5ed8ac6ee1f0c898ce" },
                { "my", "94a9523c249857665c1c417e3cacb4d25ad82a22548c4b97f581bbcfeb6884ac8362560034ee8a043595ff83bfad3f2577f1e620a9bae9fb752cb23261c7048b" },
                { "nb-NO", "c93339a21aee2421cf511c1306a481eb564d9863326fd74b94284f73987aaf3a0bb32ea823310e22ac9b1ec57bc9fbb98e05ab30fa5399aaa217504b6bc7ddfa" },
                { "ne-NP", "2b4f9d2548ee92bf7b4fc4cda13fdd8fb434bf0e42f4efc11fa95db255f8c85109413a264a56f0591838b13f373da11a117631cd8ee3d2a1783575737505fa33" },
                { "nl", "98129268beb1915a738c20789dbcf133cf3ae39a079b5a89c0c2d63998dc38f85afef6f76e83041546a5c7f01cf54dd8ab39388c2ccfb84cd8d62bda303372c1" },
                { "nn-NO", "aea9d68073ba237fceac7ea1f5c6daab0269c84b6a0def0c292f1af546c153435b9d43b509048d3249f51d611a8f745763f00a9156de7c155fc193ea20443405" },
                { "oc", "ace93cf679e6049c0ef39522cfead6028dc4f72dc2f965cfdeb01658dc3036a178f34e8b49abb42bac18b7e7d1cfbe7735f396c367180d8b0800fe41deaa9c19" },
                { "pa-IN", "2088a51c1ab5e8da67f9be421a2404e2a7733293f66b215c0a1eabeed1faae374c7f84cba2d483da343091360f0909bd01fd90422b076e125bd9ac31aea3509c" },
                { "pl", "6e64e5094b1c515db46eafa705b8721b063bba1058a5b8ff2f2a9626e2c227a535ba834c85c3527a9ec9e86b4c75ec8176a01ac06043c28bb455bbbeb79d9f53" },
                { "pt-BR", "28df47ba5b0b767e2cb797b4a182d88832cc8c8248dbeb509762b7ce7f766d9f5f3df36863ccd1cb2a9544f2f84fa00fed1f94295ca8b6e4a53bb1dfd8a19985" },
                { "pt-PT", "8757d1d23ff211a3f4df5f48791da2428b95dea4777a375f0d3fcd98147f531a94f2ef213ecc76b18f9f247c96a18b02b89557a6941f0af7df555cc1dddff6e8" },
                { "rm", "e4ba3d03772f593efcb2443e0e809ff0f6420a20c120f3d3acaab89813b9a2ab4c7789e372f62aff0f7a061cb06844fb20cc7181f2ed1e0a69bf699ad0d21bc9" },
                { "ro", "f33fc3a7d061d11e0f5e38172dc2bf68f441880c45c1e9b560da968ba0d4e3ea977619849b2826284cf9495b0177b8f00303759094e69d0057330351a4a7b178" },
                { "ru", "34c3d75fdbfbc5ecbe5686c4a06157cb9a9228a13cc576e2ff394b2f294b4144d75f07881a615d44adbf1e48a284aeef859ac83d84b92710bd24200351e48384" },
                { "sco", "872f79ef6baf98ee4ecc638cfa0f894636483b1b5318f7f1996089e9f8fb3ea4256e64a79cacdd737255b63df8a1ece1da82887f05c7e799d1f931fe84a28060" },
                { "si", "f98fa022e3cd1a2af16824a4562c424a6c984b70b7edba8661b2549a56e1c75974a31f72b8907ac6da08d9f607a1449fdb3c7ed6a931d2604dd5e7a07b6b6e69" },
                { "sk", "482dc2851da870593cd61ddc5e4c150f12083bc0a5df8f93eb603c319e7bd823f808468144a7c5075a6beae8c460d74ed95b3157e1ebf9d9caa7f105530db1de" },
                { "sl", "a55ba62d5d2ae2110cc12755ad52b0c940e9cbbee7df19fd2162c5ca9d952374a20d51f7bce3002f62fdd3972a9044703a19c76fcc4ae746496f648f0c8e34d7" },
                { "son", "ad467915ac2b2966e10fdff296f3d0e5834b990892c04e26cb36e0df8f5ec8512e4716084f3154c580fa8217d9a4192e77ebd7678e995d5620ab74dbbf2cdf14" },
                { "sq", "b3b4331f51278a1b742ee464dbf480abc6dab9056396c54d8eb7b0ba6f0aa33a70780544e228998f57e87b21aa95aa4f1841634363aca955ce710ed960c276f1" },
                { "sr", "6b5df97f79f9931979f7f0a73ac4064ab59db1b00b92e8bdb7f5b168aba3aafd8718196a35244c4adf9493d8a62ac63cb76fd83c0434f8b15ee017cb7a3bd929" },
                { "sv-SE", "15e1cc2e56d4354eb7592a2e2380d200675cf862146da309a2495dc9f1446fa2710e3066743430cfca78305d31aa539ffbd2eedb5546ed91995f07c8e49ef008" },
                { "szl", "38552e8831f496d4d735d863ccb7f867bd1d3e0550aab2832a331e7a9d58b66f85ee50fc7e2b1dacb5ffbaed2680f809f7a5f9328b4c9269b221a0f2b3ec3ab6" },
                { "ta", "21774219aae36110e43aa8eb24f2a61dcc955ba90e0d523f5a30bce15e9cd1ad954d133cddb099be4c2feb0b7b7bad6b026d17b22009bf8a51540297d55d707a" },
                { "te", "d95f8cc6418ebc02ea3f8336f51840913c18000bfc244dc6cf5ce34b823639294d751c5b1a67f8e9c4ade83c7e988a6f1e5ecb06c67e2bdb6fa81554d2bb63f1" },
                { "th", "f3864954b52cb6259187c1fd2aea9c89cd0eb2e4d6e2c6aa7d7d495ec6231fc58ec4950f8940ef9df4cf8f3447e42284d64776d0843df174acc1d7776198a3c0" },
                { "tl", "bd0016c0c6205b9761c847ea15b18a252ea4863130f2096b85c67a388dc2d8e1ec36c65c3502110259edb2d70d9f172a99f17bf7fa2bd1eb392790f4f2bc85e0" },
                { "tr", "e7ee3b61845c09a190df702e5be5a925225346056fd08af66c1cf32c7ab145c9e5fe27bb2aa28eafd3d658b8870504e3f79663ad46d4b31d3de7cbdfd07761b5" },
                { "trs", "bac9a351be180adb17591c029b0f77d27db931098deb3b0b4d995e54b5b754c6a30a7df879f009510d6449af1433c57c897dca52900aecc7ec9c6b38fb988ed4" },
                { "uk", "8826f4935049e2f98a0ac7062b6f39f6ddd13224d8ca0868fafd71604eea616e730bbbac67187b84fe8cf0b4339fe1b6fb112932185eef460dbaf6de11dbde2c" },
                { "ur", "7a9385fb1b726d02a6d81ba1391ea98c57f2af8b00d1e363c3a073db5f47402bf968626e0392be54ecea738081cdb0e3028e4ef346c075973c50343174bb8049" },
                { "uz", "91bd25e17829aa41218b75293740f0d29249cd132b458f5b2a7270de16596fd343dcbed84fec014c52ed360c4f2b119e278b7e7862ab063c9cfc5d87f4dfdaf2" },
                { "vi", "e37b9d196300bc5b5c38928064001ddeb9258a284b59e36cbfb7f1e79d5b0b1c8178a9d08b4748c6f3403f7fef1300083cd5288e5474037e26037f2d96d9881e" },
                { "xh", "964dfb4c211f1b2f1187f67439e53d09c50a822931d6e80ae864d62c4d8c81323a17b0fbca9caf8f375ee24e6b481c9040de87c391ecf33a0433e11376d07e82" },
                { "zh-CN", "3deb904050fdf3e1c7f48d51c272cbb3b795360812f86b4e75abc6f7d86f0f1f38c5fb87c17931c6f6d974caa205f9a897da52d48a202d8824aaf0372c3cc708" },
                { "zh-TW", "6db5988c4f8ac817ba17a6e83ca68069e3e9cc61b823efb6ee535dcba6727cad5f033cc6156ecc6bb4d9c3ff6d73efdbbe623a418ae89e56ebd6867eed128b2f" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "98.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]+\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]+\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
