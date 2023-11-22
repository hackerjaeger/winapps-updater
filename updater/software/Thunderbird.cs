﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.5.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "925cd000c1a7843b42afb599b9714b1b9e90486c79f5f39f8913359a47fe03ea89437d37ce14fcf7886c602a87f8a93606103c39b2252eb309c7ab8f1f443d89" },
                { "ar", "3e2c5da2dce1f69a555837a47b0ec53acaa130264f02b00b1c41cffe7c03fd493a2f6f377f8e7001f7d9e7b74c16b45f84be554b49feb9ebe9a9607001ff5823" },
                { "ast", "4c8c0902152ffe89a7a06e4fc852f9116c685c766c648e4126f09d36f49c09ca417728194f8ab3533ddf5c42bf06487f393d19549771199906fa2e48d052ab85" },
                { "be", "7763ce7dca0c73d816912ae58344e67f057359fe728a12f16950f731d41b9ec3ce9a4fb65d7bc8bdb3c521882051e1aa13bef1338c6d03277ab23e3e3742481a" },
                { "bg", "45fcba414267ac4042af253db3bd444268110049ba42aab87bdc99b2acf3d4281ce9c27a8ffbfe7d682ba9010af4b4a11f4b7988735bf7a2920bf79a1cfa2883" },
                { "br", "29571c968830bb7771f8f27485750f18dead151ab60a1ddceff495ea33142b951c9b967d6abac9c0d01507525fe7c2de16f307b7132b14513f4c6c8bf4fc9e84" },
                { "ca", "f02b857bcbeb5d46ed2f1ba5dd153cd92e53d8b7af2a5937da83185cb5ff703789365db8082395de59c8dc66d8788252269f6b808edccf06cf4a3bfd32d63f66" },
                { "cak", "ccdd53a3eeb6771f0e616f77488502cf091ee00be0151f2ef0ef6399b5f7b0a081eda7d748c5f8e2af3278a0914662c521276ca7227c048148e4d270b800cef1" },
                { "cs", "84cea39ce1141cefc5be40e829cddd0fb5aec78484e78d1f9829e03652cb80065b6f820520c32efb7e4ec928a8be787dcec6fe597c555b7cf686650c905352ff" },
                { "cy", "1b749238dff3da37c0837c34731d95dff0e5a4075c5211ae8a57a188a36a450e9cb62a24ce0adbfcfe61225b4022c81cea19f64b80c69a7681512c8a0cd74cef" },
                { "da", "f03ae79e8328c118b12eb9a30c884d24b657dc957588ac1a58f6f852b44a0067e6207b8d1c2920208dbacdf53b997d4ccd5968289278f4e003f7bd97c4626b7d" },
                { "de", "3f8120a7ea7a9ce1853d542795c82500d089683b87ac63554ef1c7c7756844dbca904772c4a1c7ca966db383c9ec7034aff1d540874dc9ca3cb19580ab04b475" },
                { "dsb", "06bf35aa38990988c96af4d4341461a3206e59af7a3e85ef14e4245a81d0e88511834cd46e7495258bffce1924b020e157d475823471525f26ce434affa9cc58" },
                { "el", "b04c1fef4587315f327506584da7481d2664634d2f856d56227f968369698cbac85d69cf568c3785b9d74329d20c1ea066490d24958aff9e81879f26ebf3d100" },
                { "en-CA", "fc4d5290ed31bb8e9810869b2ca744bd05cda7908c780a1be72aa490e346cd260c2f89292e8263c30625466bb5f7e1f712bc267dec49b3f97f8eac0da7b4a37c" },
                { "en-GB", "d26715a17a8ed5745f15b5c90971b36aab7629bf0c05bba011a92c0d0fee49912f938e8774f96f11ae8653d7458ed705fb1d76877e221b9be106481b44ff82b9" },
                { "en-US", "336218498a6e14439ef54b31e09b3a5b47a861f05ac49d209f565f5173bf97aa0bf7def0aa1960e9d846373783c51a6c2ffe02b56b705b2e87c38cad35bc9917" },
                { "es-AR", "7669fb3ed649aa3a16ed256fc05faf5f818bf0d9486318775a5f6f98ff85232d9d433959b234199bb09c01071e0f1f952d1110af34d907015ff76ced6a4ef566" },
                { "es-ES", "bae8536ca934d7030eb4fec3cc1b630e61ff4a2ea9e7452260c1ad2dffdba12525a0d3786b71dc6278779c7603169fe69edbf2aa2567a1f35682896ca00ced1e" },
                { "es-MX", "d7a5c6ea808b5fec5f2e587a0a40bea534855fe6811056aabd3f7fb763525ee95cbda1b9905b9fffb33035d1c3335d7fc7b5db119dc68e4e8bc5fbac053214cd" },
                { "et", "f99d2fec71e0df20f1a998848482879e932f4ffd21d80d4d58c119218d26489b85ab89abc842733c7ffe35396948b87b839fe6da1594c617e2fd996dbf90b285" },
                { "eu", "637959d57b8265ce9262a13a1c9f71085057a9131bfa1ff716e87892f80aa1c1a1a9edd93a3825cf143b283502e2b074b8848c87f45bc743002a14dcb4710bcb" },
                { "fi", "9ed32d5b3d3ca92dd8a7ef73e2659a6db683b9cc9ad86e99ca4469f62ab11da252c047b072e5cf9b66132f463a9e321cc4208752c639955819d11ddad6ef505b" },
                { "fr", "04debfe6803befd4b83657eac426c72e9c3d45b1f4fa1153a3aeababad6acdc93a41b72c8bd12f49fb239ba5c835d111a2d7e2044cf7cf9a23c2408cdd43a08d" },
                { "fy-NL", "721650e171d9304a4881cbbb76f73843cff162fb20d7c538048d59a4cfa55f0037bf2c13c63466a51188031f5ff97b9702a2aa185314a0cdf94a5b2132a9203f" },
                { "ga-IE", "985bf0e395bc3f8ba6eafe73909dee5613582b83deb88613f7e771a925597458152335a3ba823f3f9f152e1dd1bf6b847bec3c4c01e8a570a98eae24dae3e6c3" },
                { "gd", "99570ef694b7da49c515e50634734f71c4cadd36319f0b7078a5baedce11447d7a815bdc8460266311e479811300befe188226b4d0a8b9054b83b8fa1db7c6f2" },
                { "gl", "ae0883e2d1b277fb31ec1cf1212e9deaa56f2d42633a323102378757df7972026cfa2695dec712e6e26212c3dc94d54b48d015d09719240c692a1a9b4ef572f3" },
                { "he", "5ed23e68bc1c8eeb67f662cfcbfb9bd5f2ab70e1529babf08dc86086c3342525c05751fce940ae2b26bb666ab9e6e101eb2ab158657f2c8d389f7edd34687470" },
                { "hr", "711c04145124d072f7ea8a5831cf7f903024a6e6403905ddacbfe81abdd504ff9a666ae40b4d2e95a16b4b724fb1fefa768c028270b548839a22a65d1e155ded" },
                { "hsb", "3c3181455769444afb729600174d40d90e5ca286c3900e65d34b40ac4180b3b8a081299fd7800f50989747b798549bd8e3fe9c198256f28da3da8d20a4a7ff16" },
                { "hu", "8b7881e4abb6729053a4ace5b69f67d3463c6438c5bfcb74a9e6b645652cd96fc16c8544e730e3725519eaf7faca125f463b166e3ad5b48044755608ca705f00" },
                { "hy-AM", "ba430e0de0a06fc262af07a78e84303e1666fa09c41f0633c929332c4ca0b3340c6d5861edef8fa7feb07b69592236256dc78db13606a87e81211ca256e8f82e" },
                { "id", "ba84b9640bfa69a0adb24c84d863bae1063094e8436e9fa50b23dab88fcdccbe9f5e59c3f5c47381fa3046b5ffe32441f865e56b54c4ec6396fd9300a6622e88" },
                { "is", "8241e436c5c4e81fed0634de1d9527c0e3c344148ae9de0da3ffcf28a626373792ff5bfaaa3360faa4b15fc35dbac62d212e0be6f5c0d7a7a65184a3e3efc58e" },
                { "it", "c84d8cc22a1064d998b7511c55f4c2a4ebe18ad9c7148984586e793d0cdd571a36ce54866b7d804138090b9c5ab368104fbad54148d233300752a015e3c91838" },
                { "ja", "4ffcda9ea738d2b62aa23aa4998afbf93cb6126cabf9806ba0c7b5b46cb9bd3e7de74dbcc003d995b092163599eb640da3e0208a195b1798dd7778c41903e027" },
                { "ka", "b929128fa1d5edf3a186d347bae5be51ba09e756a3a7c4c5361fe6b2e0899d8eb459f9153322f1404103bfaf52fe3febe705956c05eb9fc3055cceb778fc8ecf" },
                { "kab", "35b1fa23778c2873eeef1d1d528e9ec6338738543dae526c8f791321199cf9d0d92036c0fc946654e73f3f6a2566086d29adc5945eb84a5b64bf579b0bcab9b0" },
                { "kk", "ea4ee33146db9fd985e88ceaca5037a5e9c96ecdf1d1728a7271dfdffc601997ea8916f6404c24f17f27f1e940392f82d165788bc14b281988ff3d19f3f0668a" },
                { "ko", "51aa9d3f8ff9eb8a6dfd329921ee0724126d623a30b610e4caf20c006e7946b3cdb9126f62143602cb4805090f0a1042e8c11eea7276d81cd99dcdd8337961e7" },
                { "lt", "15bee2dec535d7f9f03fa3a92f474dae22217acf7eefaa169ecb38448fca5c0572c87bdf03e162da970c022be30a7d422ce9499b0ef33db43510b06e88489dce" },
                { "lv", "973c6308eb2345e177c9e2ad9ee996a15efcb8ede30bdc90ca89b9ee5e2a6cbf9bcb50e3924b0cb85fa2f0ef065c6ff466db928985971199c925d919d9dc89f3" },
                { "ms", "7516578c8fd84e16a18f64615219826e41b9cd425b49b626c948ca9cb9b9508b4d474e129426a1568f727208f1f91bd705e7de3a160a2859a21188ed0c719fc6" },
                { "nb-NO", "8d8de2426a0ba20b28488ef714a49c676cdb7d70e4eeebd0e16ef06222cc75f9677720a04df4d12a96cf912c8405cee9b4c2e901d605abea706faf3b99e64cd2" },
                { "nl", "d38f13c701fe94eaa5b25106b280e4bae8c1abf8911ea7a73af0c8dc8ed83272cdf08f418faddaf7eff8401be5b1be0755e82d0b0b1380d37627d626c88a30ac" },
                { "nn-NO", "168a22200dbe58aa0e68e5cdd75b84e483bd729a5034f530b00a108f328f53572c14bc211b6c80c37ff7ac314ceb1041794b0e54ebdedc6269652486f43968f7" },
                { "pa-IN", "c9fc684deb14c2d351f791e98a5ed7bf3d078dee600d98f9b8ca04af7348c406f534773b91c8edcf789cfb3907dc1b006b2ab878200fe9b48f7c38da56851132" },
                { "pl", "bc9e064e20a85c86a9a20dbb5a7b3be3577194527f1c4663ecdf84bc98420b2abb55b813db7f96fe41a41095ff2240bb31422fbe6151ecab19d37204083e7289" },
                { "pt-BR", "c9422fd02d595894bf1ff778eeeb006764483444551abdab609e02eaabda7b46a013af52ab2794a7419a40ed1a4afad961b23f858e587eb9afe574d466baebf8" },
                { "pt-PT", "1ea2aa6d76eb9ddc9ff3a60ec81714785869ac01fac8b2901129ffe39f98657c6a2e52833ca59e498018826a2450a5421d84732bf7399fa911b3d4f0cfc18268" },
                { "rm", "e81fdf22fdfd659b676907d74869d78f287212f8fd3b2c7c44f48135f10dd0b09646b95d5fceb1e28c2ad5a6dad5b3598421f2c8959c3485d6bad56a67e232d7" },
                { "ro", "1abe9169d400fec74a91d451edee83c2ad1b8e3e09cb275e452dfd6358a475ebf06ffcf2dfd9b713d70891ad687e84917448a3d0938222cc1a013740667b7875" },
                { "ru", "31303028e9c437ddf45340b8f5553a8a810e45850e0c061ad6fe1cf7b45682f3c28673e338afe301d9ef2f69246d4c6bd89d2c67a92c11cdf4e2413cc31a92b6" },
                { "sk", "4e33cc649de85792a2ad25c4bbbd9cec928723d8dfe7b95605e75ac7c2fd8b9541a9358548dc4defcfa8966bec104f3ccc84620d96a8e3df19ad7918761d8249" },
                { "sl", "510414f5e62f2bb38e4f4b8f1137b43466a8c7c0b7890786c53049e3268c5e0683509a9e4aaa2a628ce83b1d3a4c2f2a0fb04adf5bc6969061e15954d4322575" },
                { "sq", "a0e5729d0dd21062c10a6af8f725c3afc5618de529fb8679e491508853343db6166b44662bb325afea2b9102055869898cd9c22e14f1e4a2ee680fafd468226d" },
                { "sr", "932865301ada8bddf8d9c2b39c2fefba890d2d18ebcddf54358a1ee5014c81ab66f246bdc83f58efe99c255f99bcafaf7ad13eb4999014398a500c7dde23fab2" },
                { "sv-SE", "1e4e9b46da0601ff10fb8c290e878d7264b2fff2adc40a25add95d56c0dd6194ef288bb1cfef83ee86a2fe973f077b7a1970291bab483face3fc2201fb541eab" },
                { "th", "72684708cdbb015d8b8c1da402e7129ae97daec0a38bd2148fcb9c65f9b2d8ec65273f8ddb7d2bbaa7eeb70e3729bdbacec344d50a5a7b5c751de28a24f143c5" },
                { "tr", "114e741dd575e7f746f546180d4fd354d2ffcfc8e98dcc45ddd89eda2ace6f5a016681fdbebfc1a40de896ffc88800bd4237fd36ecaeea73a15ff53d270a458a" },
                { "uk", "848b22d5e33aaf430e76f627b0c1a755e981fec039664ed15d18de6e7d8e66ed2fc972250334acf9f722f108bb2fff095b5dfb2e7f4428cafe66c209737a9bcb" },
                { "uz", "e8ba3bffe0a02fb9eb8121020502954b9ae3ace58443c7dfa0d79bbec50201a923c44e3d184d3a63cdc9ce5c01e51713e593332813ea44246eea58f84011f367" },
                { "vi", "85f25789e093f75cd05a7d190b31d86598b69021a325628a889e6a454fd1a5ea87f11f32998d5603e7407f7b40595125e52cbe7c50e9a99927d99587b1f36f25" },
                { "zh-CN", "bae372d259bd4bd23930817565fdc10eaf6fd357768f79429813dec665142cc4765f49d225e0756808ba44478f7f06b6821a9cd9c11cef3249ff9f2e29bd6f97" },
                { "zh-TW", "38ab4ddac129bd0a08f4eff5ef605e6b59fc893de86f3db39fcb7bf34722df6b662afda8f8faba51db6e7c7d420159661e819da33d11ca805bdbf6784407ece3" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.5.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "c09efd7a86f0340664802a47bf04a5e16a289ab899e077f32cfcbc396b69a04bc09aa3d0df526123a0418b7db814c4aadc8a787676342e42d03d35a650b4d560" },
                { "ar", "b556ac1127d10302cd5693fef30537d9a8e4f5361cd6287d769151ef41ceed270ecf1d42e4e6621cc1d6789dec1663ad4478a854890fcd5de816b8e5399815c9" },
                { "ast", "610fa0c3bd6a8e6f16588f2bf6d742a89922509e172448aac05dff95d273ba980de5b7c5323a523680b4f7eff43fd989a6766a724d1f39613c5a7c0cf2cbe428" },
                { "be", "f55fbcf8e9bea55ac813d5b170fb89863abe4a00d2c646716b3a8ed7bec9ab5245ba2ac4b7623a42144498ad0027fa751d6a57ffe3001970cc61f54dd247ffd9" },
                { "bg", "69e74b0d1018e813d5c47550ecfc260cc564b8b6fd293d49f6aa9c1d22c5852946aba7060ed632307c05ad86f848578128fd1b5e770adf0ac03d5140863df6c8" },
                { "br", "16a55b977cfacd6b4b6b608176a94f8289c75a82f0592fa9d4c5e1d046cc5a2d6bcbcd1ae31d0f1b1bac08a688a3f91485bbc7f8bcdd4404c845d4865b4bb9d7" },
                { "ca", "4f0405274fb9cb397721160e8b2e44022f84451a1a61db5ac9c23f073f2cfb0ee1a95716c5b3dcbce1cbab524aea228b5ffc895729ad3838e3a0eb999f43b3ad" },
                { "cak", "481d7e7a74ea46362f6a39404159a06315cefedd7e31920bae73e76f4a9aa9f5db3288879fdbaf3a521ea72d4053629fcb62077f7f5ef0a7f7a622a2b0138482" },
                { "cs", "c23afa0512293745718f0b507eac61ea0dfa7f402b6419e52f51704b18f020e876867015df302c825380039c60cda2ec1ed06d7e1d5eaefb3830c74e99df54a4" },
                { "cy", "53843347cb02b081ba996050d23a45d11a11b45f2934a9b178703c08a9d4c44eb36f77e87f56cebf360651b026d33d581359991ee6981be9cbe0e20bd0134677" },
                { "da", "4ac3060def397658e1b6be261f510a14a845d679e7e3191e2332a7c74482763579f9d8f440e9f208ef4585c5888c06999e5c993045b5b47d5d08787cbee58201" },
                { "de", "49a1fd27a6974a95a54ebbf523b0eabe392a5ae529ed008c8fd580d10400ba99ad26a7303208f21925eac6c21a38d372ba666fae563a4c33f2e14c0c4b81a771" },
                { "dsb", "6ab72541103905ff628911586d6749ec6e87c4bbefc5acd3d15638608db0497b8bbe893a6b92ca931ccbbc7e36b821253b2ea84331635f384fb6b67a0e0aa543" },
                { "el", "cd388ba730ff90a1b4d731032fbf146156be5cbeb642b1980123f2398065faef42b75c6a7b9eb0e2e6399a6259d5cdd9c958a164da77de4307848a708821df9f" },
                { "en-CA", "764f98788d8f72877e6ab0236f68c5d5f662a5d45fa86209d2261f790f31c1f24ffe32ded56d88bcd239b4e8746588e49d49de348b9e4b2b640172f695bb879a" },
                { "en-GB", "cd0a3390f2faa712ddf2a41459ccc6a5fdaaed5ed89dac75e4aeb1a37434620796162ecddbc65d475ecf04f4e290794635018465666dea8ba8d93110477901b9" },
                { "en-US", "8fa6cae51209176e1447a4f1ece1c2cbae19263ac39665785fa56feefc0a6700d85a42972da03498f39a867f1f79dea26f930bc7167f434dee164a444067c604" },
                { "es-AR", "d87f9dfd401e0f31b30496e3224453750d16c357e4c11f364e53443fbdedd2c10599cde1c9efb6fc88ac8d9ced78c8b172383579763f76c6234eb1ee9e795163" },
                { "es-ES", "87fd7038fbe54b183f47e67d9d0aaad579c37fccf1c2d9c4bb416e5f6b9dd0e4bf5b33943c2d2cd126d8a8a3bd764754f7360ef61b5d3ea4a28c30fabd8de2e1" },
                { "es-MX", "182cb46b65fc38742adcccb3b8eacba1cd81e3140d9819d0e07d7fcf38abfb907627e0e679cef42c496ec5f503e68508433b211a9b722236ee28f57b3cab5ec0" },
                { "et", "3dc0bd497098f286f450e8521ae8593c3b02e4c9e2de23b66c2f351d1ea7de5f23bf3bd12f4f306e34caffc4f6836c5e9baec1406ca3f51c7eadd3eb781590df" },
                { "eu", "ca0da8fdb90075a2f3f62beb09280c4ef4df8fb37cad47e3abc0a3488f8da9a96bd34a0c9a9346eb6f5683fe10946319bf000e8707a57a26b8230ae921d173b4" },
                { "fi", "fb3c96d4aaa0a90cb70ba6954d054858554e9525f763cdf9670c3643329941a5002d7fc5ba970ca69b1b3837294c880c82330f013631a408f4d89a16c2bc0665" },
                { "fr", "fa7fd5dd18b285ef51ed5009cfcde2c7c8f0ffce4e34b02050b977c50dc5fb65de4d28fc14b4484d1198a3c594909b265ee2ec57ddef3ae07a0e39919c33291d" },
                { "fy-NL", "8da893b121ae9b313f9aa87777dc0e6b9882ddcc8f1d1a5d5afa1507937cdb42481abae50cebd2c0d54f4e58b4406ba2cc6aca91bb4aac4bf4aec1c29bd1e733" },
                { "ga-IE", "b0409c6a8221f22661f659c1409a3111f5b08f33c5133d2ea57077b24a27e302dbf1a75be3c8b40db3719a943330576e603f923a90767348dacc825097e7550a" },
                { "gd", "9a205792e05fb61b28992eec05b7b701c985b547e29e9d1419e3d6ee9bbd7606761e71ef6cc101ef6f4aa0f91bc34339f4dfe5fbc6a78f4f0723861c73a7e426" },
                { "gl", "876b8bb8c33983e5786e61a57f3f5bf98d59c88c17c8c989fe0b7d6d96f9c1a45d81f3a641622e5121a2456d7051445ebd6debd943dcbd07200096df54145993" },
                { "he", "a6ce4eb8f0acad06e4220538f5b1a42750bd6f480a4b3f498b756d72c258d6ef203a8267525b02747fbade9c266bdc852a2ee4bd76a947ba47c3983898a3ce42" },
                { "hr", "0e1e7a1e7942edc1205cec3e95636658e872f533bf7b2405a7b753761940e9c4f8d902381f497c403909801acd43e2d8223a259709f9671b21eae74e1565ca95" },
                { "hsb", "1aa1069cbb06a7d6a92b2a9642083932ef0c441b9596368a8a8c86b06092bbc9dc4a50a95a2960dc77728905db9559970bcf9a02c0e53cbe151b412e420d10ac" },
                { "hu", "ffc77f5697e6c39316855ae13d41ca73d481ec5293a7b047bb1177ae127efdadf731d31706abf26a02f91d65d70390b1b85f7d161d65c6132152d89b1239be31" },
                { "hy-AM", "e8438c1ae37c67323282848e02a05404e6135a86de7287e359710430728f37ccafa926f247307c866c1fa5fe28975e93ea65bef5ae77eab42fb8643cc964981a" },
                { "id", "537ac8e287abb80dbcf6e87f8c5a7dca9be83705f01ba23ebdbb4a427c726b8b597e0df3d90928e7eb592a1e6e3188e3974204a9c3320b255aeb9e71020bba75" },
                { "is", "f1773eeaa245c3d523db397af53f236933585ad594b95e72bc50ef199ac16c5d054a37d29946b2786c0a78a569a10bd34069581fe30f6e206e6ce5a8998cc96e" },
                { "it", "b9ba96546f5ac791ac3c650987a0e3f8632239c6ac3980c633b1ae4298f8ecd455d8d6034742c2f9cdde49abbc5e39219d5ea3676fe8423ba823cb46cd8bd4da" },
                { "ja", "ae28baab4cb13a73675a6156a52bed6ccee3b658c204489754df5a06ce86ee0ff3348bbb569a96e7592359f714265d1a91f0f0d0fb5538e342aeaa3e0f70d2b3" },
                { "ka", "ccb03dc544cbca73f5a85392af060b1b5dcb358ece12a869ded235e9d85ed52d6be55d3bb4c673f3a2bb22d6c0be4af5fab96555dd3c6f3b5150a49a6e47ad00" },
                { "kab", "6f2d7c9a52a697b37b3ce74157f5e3c5cfa0535eeb434ea4115c9edafe0526fe5e67887d00d8eab5e1567500fb154b21f89fe05c0a424784ce341fe2db004403" },
                { "kk", "c6ae209dfe0e7f217f7a06793d5ac128cb96135c42a6a8fdc134d6197df5b4840274b808d7a1d6dfaa687f367778373f2983a98cd18ecc3f8b383d086d0f7d3f" },
                { "ko", "c5f96749e3bdaedcbc94a8170d307cb5fe99ac80279a7c2f7929f30da2848bf56c7c772d1d875d8f029a9efea4166fc7a8745764d7ffff67b04a7c398be56f3b" },
                { "lt", "d3d9d7c132041062f0673a8f595ce4425f3381e25262f5a0c8c5edda926ad428a72c74ceec4572a39896f5d621b1729851e263cb1f2d4f2a977978a2db7b51f8" },
                { "lv", "80ee82504e9c1eafebc4a9b1fb1c24a7cbed125b62bb756c77047fba688441df0f1f03723334fc4f63464b5466385f83072680cd405cf3f1bff6f1991ff6d375" },
                { "ms", "d96ca80019f8d25d9439292cd69ef644386b01d47eb3b163c580b21dd88920e78e018918a2eab3adb5ab8e56e5d00b30aca9101349650527608f7ddad2fb4dd2" },
                { "nb-NO", "bb3ade9bd096782c5c73ac5733841436140773ec1cf0a6aa34553e8306164602e0f81a8649417264dd202c63703f078a84a3abe55f8451ce260e15829a86bc87" },
                { "nl", "1c8f8c11add033038296b5ba163fd49035bce07a977b080fe1df17daf9abbec19e13d49f3e8d68317bb3338cdeef271cd03fab3607f36bff053698fbbabc3d35" },
                { "nn-NO", "79461a72a40c82a750bf138ad9f1f2d99995f8fa0539f15bf9b1dcb763bc5796d7899b557c494044f84299c454714285c53129a0fdbb233fc9e7c5400aaadef0" },
                { "pa-IN", "9bb4c230b4bf07e64cc95020f98ad03a97e9740685f202f0819dc4b6c29d14cf0e33bccbbef450b6f1c5ac60b3ddc706539a05eb5146a343d658d7b1b513c110" },
                { "pl", "7d5bb09b348289f22cf3b6543393598920c9d8d9da9c6394afdf80c5d5a4e6da202c88b93900987270d8688ad675b4393e8a151a41ceb4dc42d7aadf0a9d0b6c" },
                { "pt-BR", "0f0254f52bece417566b3f64505e6d26b4b3b6de6812734f8ff434d78893b43be21a6e29e4f359c70d8deb5037e12ddb08bf6f8f60ff48a4614a1b574e80a16f" },
                { "pt-PT", "b99b86822107dddcfd504eb23792cbec42ac9a7d13506402798e43a59908a555c1b73e28e34621f96cc61d7d085d2bbec5a1f7cdd4ce4dcc698c6a82ccfacad4" },
                { "rm", "53651e6a890edd84cc7ad0bf6699a4270812cc6b23810a7323d235e660bf97f5577f6fb1394ba897e2a1ec9289403f8fe57c97517841f370adca8d48bdce74ea" },
                { "ro", "5d876965a870f47fe2c65175fe3d916acfb9d0f1d91b6e758cdc21299d6dec26358b582e06eca907c8ed6310435940ecb8257ee93b2f587f0cbbe4346afe82e5" },
                { "ru", "5eeab280633fcfded90f4fd644d3fe9fb10d1dbd7c986399948c4438e1c93e5d65012d75518ef27a242f52f7fecfc3f0f714389542dad104f8e8832ddd3a53de" },
                { "sk", "225e82344422a10cd5bfde529dcc7e77d722c0943d3186bce3fe308f63ff0fa1ec39dbb27dd06a722ef2069d58c832a20e4160092788d6528166bbe33ca5e900" },
                { "sl", "849a1f52fd3fbcc70ad2a38af32641b14414ab2a6ba5f089eb88f016da23d473cc0f68175a15c72f3a256bfaa9f08b1f5d210bd73cd0193b97db487352116369" },
                { "sq", "202dcdd18932a51ed0bae086e1fa31170fd97896cde7366b15585b56fa36a5324dfd6e43bee82e69bd651804c37315ad3844a90715123cb8488caf0765b9091f" },
                { "sr", "4c218324eea3c475bb936c713740ebc316d62b4a256fc6ebaac2040b96392fcc5be4a27c95d84d530376fe3089531aba6a908c24aa7e76e72f41163be1bca14b" },
                { "sv-SE", "aae345bd146906e7326ff5adc8dd21c84f54174ccfca86ca5b540f9b9d526a7901941c58d9a39028c3c95a11cdd3e76c0e3d7c3170c82c9b17af2c25da9e22ff" },
                { "th", "3b198967b66cd270d1e721ac557b6e77a4ebf30abddc6c9c6e3122e5db13a9986f7f4e643a4c05b2ed683a9947af2dc9838d370150cfab10bafcec29bc679194" },
                { "tr", "6f1a798cfc6c24551f8f209fed9987ccdec2d68f39e8e884896e979bb53ef031a3e6bc795d4a2b0167a2c1ba7365bff07c1a83738a2d56a1b8dcd8b69ed8ff0b" },
                { "uk", "6e95275d0cd5b7e917423ba39205aa62d56522fbdf6642b933a37fe8039b232403bf6aa2b07c1a3db3cffdc71d35acb3bb0cb7f3f9b37547db32ae4a0c340bcb" },
                { "uz", "349e15f583d3ebfad66520dd1f858599fb80069ce66b8af0be9c7e554ee4a56b8132e7278e95d6d35e819aecd0c523a73dac639ecd53f0f3f7bc2dbefc9ca1e1" },
                { "vi", "7d58035b3fe8e2ed944bb1ee2f39b58587e853e726a8cf2d6a31dee8a0752d810c0088cf15044385bd791fab31b9d2bfd5c18dab8aaad115f6d981aff2d16c23" },
                { "zh-CN", "fa12996dd8975609393dfe1fc414786b6083f8d61f201d165e1a0c34372d549e7aaa252118feaca0a2c7d01094342ac2fa78fc76b3282f819ae092c122dbca31" },
                { "zh-TW", "10a0bcdcdbab0ccc494af3c9e63bd5bbeb35f3ca956c9252714e3fa61069431467fbb9b6915cfea64fb6ecee5e19d3645c7acf655db233b967a68c1170c6cc5f" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.5.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
