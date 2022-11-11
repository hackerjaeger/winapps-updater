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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.5/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d5296a93049cf1b21cc1266fe7539d731a8b109bc845854713bbf0024deaae84c92f4fbdcdc3a9bb1402ed72a2dfd4afb32198c86a86025deb6b54d06f85d594" },
                { "af", "c31e25c400fc757b0d4d6d31d104904620908a91ec257ad7902ce55073fa7aee6ddc1183fb0c10ab0daa9700763e630a4bff8b564bfbb35ce2c0e48074d21eef" },
                { "an", "0fb0c190962407969b25c2a5036edd624678531ee3e5adb8c9864bd1b4fdf1bc6a22ccb7b3633d1d31a1546d470500cdc5fa1f70d31f9d4b2f24e4916da9ce03" },
                { "ar", "d884fe0c8679262118d17152a9ad7d0a9ca340dea0499ca266292da3980200f29d9a0a3e98ef3840d366d439a98ee958f85762b5301552e44a6ca57b405de9da" },
                { "ast", "56e6d6b3d4f145e2de632e578832923d4e24e8a4f6aea5f4a870e0fbd492af23d776d24e2e4971bc47e4192b62efcd195d76bad206d6826e232754dd12c86b6a" },
                { "az", "e25f072bcafcf9d13851e521c4d4f1003b8354c837e2b36fc0008bffe407e82429c30b79cc582a6b178f51a2efd4d8cf2c024d15c2088d7a534d7f5a60c46072" },
                { "be", "5370f26726175690c52b858739edddc0932dcaf9666e2d1fa06fe755e08ebca6df1672c5957f637ce4a2bf3da5be0ddaa48a4d29409aea472f28a80a3b657491" },
                { "bg", "ac1d336c39305788445843865ebfc2e423887ca84537d5f858910bb9de046eb0aebc7bd94d2ec5a041b4677843a0c8c040be85b27aaf2c2482b6fa3d8775941b" },
                { "bn", "85b810b90c11537ed21cc3dea75a0982b067b20aa58d4907cc743f0a66d7b69413a560c527dc88aaa403620a2bca37be4737702dc580d9da0d2a67d3504cf577" },
                { "br", "3aef9ebd7872a7a8e644613f39af8474e89d6d094fbda3815998277f85c3975365465d600c26370a561df6e0c14f28199a61ea2d2488c261d9882e3947b669f7" },
                { "bs", "e0cdf0bc98423645434f163ba4f0cd04dadc8f8367bb08449cd8d2a6f42e03694a039ffef5d7ac5747423611752b0679d34ee2109f1a8d864c3186c74fc59518" },
                { "ca", "dedeafbf11710dc27ea16ff968ad289a33ef9cbcf79e290cd4c5dd33f1a7db870cd90c85797d246701d5fa6d8dd566d6a1d4960fa9674699a755894bafe4cead" },
                { "cak", "f0832d0a579716d477b3cde7eb741d97296fa532c703b4f9d493e808d00b500a52f14d1275e68c84fe022d7959b56fb82fa3e9446f440f359ff449f30a1b28c4" },
                { "cs", "d7aed5feb966fa626055804a9a1a2f87964f6338497d583ae390ebabe827980d92557e79b66c7ea250a55ac7282e7cc2446225942053ffe8076539fe1e15db0d" },
                { "cy", "4bc74593839bade5b844d33b601489928d8b52b658b299b8b549934c0cb373569894f8f61cb0ac52a055e0977f0f57593ca4c663369c11905e57adffa32383e0" },
                { "da", "67657b30c2232aa70db221c32049ff908bd449bf8e68e3ff33a4ba3560a753f76b90db806d27eee747722edfbca2eebe158f8a85b55fbd746d8482f5eea466bf" },
                { "de", "46aa2359d63dbd34a66ec7bf2537e0ce593135f7b27a3fc6cc17871091a5151a48eaec096c7a7806c5f60a315e41945d8aca3938a3d916ae4821ed36e925c6e3" },
                { "dsb", "82b69f39ef23200ee834950e232a47ee29ada0f7b1e733e1e8162ac98e4936e30b8e312e059495bf3628153e4c7fb3ae5a26fcb5631f2b20e8bf32887bbd5004" },
                { "el", "358af426e2a568a99c83c8eeef1ea622bf8af6ab00b42e5fe78fdefc1c671fb75b3857076a5044426a2f2971964dc0ce7a620d976a5cf0ba92a7411030021ce2" },
                { "en-CA", "a6bd49a0a5a1d04ac47c5749c423f9eb34bb77d5f7e34e612f3278ea10dec1cf9e08846bd7f7a5c98628831c1864b96d41db88151da02b1dc7364d59b27ebef1" },
                { "en-GB", "d022967d319c4bc50be896b28f8b3a6def682132d28a974262ca3d571143219a583b1adbbb3086ba54441feccb9dc519df13dd9a293c8252fa598aa8e4049b04" },
                { "en-US", "03987a02baede38a6b20b7af4f9d08258079bf0da52754cbb5b117f72a0f97d1c304d3cc60fddc9c9e3638cb0db5494414cd7f8639340fa482cd3cb6643649ef" },
                { "eo", "805a0383814e96a5ccbd0f10634794e84bc3025f497341f894176206c6e8e2d5aa5631360c6b0e8dd63134d94b3f0055a264a35c2ef9fa8f294ec19e88be8810" },
                { "es-AR", "4525eb1d0dc06434f953b92ce7a9776fe2047c20c2ea06b7331223c410399e8825817308113548ca83dad444d61371ace09c933f5614445c497a44554baa9ace" },
                { "es-CL", "dfdf0eecef2ea8dc1b822d862604f2853164e240ffbe2e58df7ddc717e30c9874b21e406e0ee24b8fa4fe05e15f063998b9596169316184bd585290f602f7315" },
                { "es-ES", "2925050d31e0daa4d80837a43c878022942289e69cc8d72619e2a3601f0e6df2b5a99abd8a6a9601a4e93db9408b3beed2db750c6f8727444e1e7feb3b9bcf5f" },
                { "es-MX", "f519f209ad82cb252b326c9bea528c6ed373f4e261106a352ae589a7ba9e3c97b5cb2d7ac38d36e626fd80cffb91be91aebe98cb192a94023e4b2b1f5e370278" },
                { "et", "7973b7663b9371d7a937e859cfa6083f3af90b00ce678fa5ecacca5e8b966a47b0adf4b2df59fdb81365d0941ab0feb4e643e8a0da514b43ba069a56877cde47" },
                { "eu", "c2954a5902fa6c4f3fce671e88545a5a1868a67c62ccf5afdd6df54316859a660bba938ebc493011e69540fdf19af51cde759ff31b3af55a348290179fc9d67d" },
                { "fa", "d442820c5171874ae985d5cb5c319ed6dec1f6c67d23234877aebd3fde1cf85f7ebe15c78b4931a4e5c91a9c42f4506e91424105824f27ca7e0e3b8ff10898a2" },
                { "ff", "59ae4ad185e94ea98c307d226a9d7fb6966d22721f8f96e4918ac021b8a288c9abdcc23ac698de99fe886b4f273fd9ccf3192bcb4c8e74900dd5b484185b5929" },
                { "fi", "67a92a042f807bfe580979d37df03c5f64182d253348502a7cb956e902c9138f1a6ea19afbc81c4bfa0a959ad1147fdfc4625984b46a16136f947b26223add22" },
                { "fr", "613d6b2fa5ce3ebad5bdc3f1c8ce14ff6d5697637b77e305926910e864e832b96a895bb9aba2b7fbbbb9035fd121afed2d1401f02adee55415c7e768657cfb92" },
                { "fy-NL", "73d5b6875fc0de04fa4958bb755f74c618103999ee9017cb93c107ba2c3633bc3e5872c32460a69a588cd9edd29317510ba6dcfbc5408a0d71e49c0a2feb4a56" },
                { "ga-IE", "0762fdf4196d5210568832d150b1d0c41c7321715f9c98b17ed1ed16b398609d05f102b9d08f38dd56219ac69b7c289e4511cb138ee9481e8368691e154bcbb9" },
                { "gd", "77ca24620ea7344816ce9018e05bf43b7f374665a199ea90bd3c63d0d12c7c7af653b9ed99ab3eb252f4381ea6f52a3d2c26a76f3d89d59f810ae0ffe5bd1e58" },
                { "gl", "07360896569da07c700824782e9b5cf292d0b0a87df41aea0005621ffd4f7ea5de4ece56410b67c130cf5bf25a871c7cd8cf7999a0ee0bf1edbf1c535fa45c1a" },
                { "gn", "7df1e0448d1f3f554e5cfcc4e4a826c88507787066e1d018f77ae604135881dd875c192e5ce9a202a3f90473dd5cd226ba36e09994afeaaf5db147df73de431f" },
                { "gu-IN", "5035f001ba2d00813baf4f923822c552b5063a0df58e99967ae643e5e3ed15cc9ea042944e3f9b11b4d6b0a9629b5c535c1be421abd6c463654f575554dfffc9" },
                { "he", "dff71e0de641ffe5c1c9e24cb879fb276bcc5cef2d9e43c6a328df2d362866c015510485aa28b0cff59b4d098ab94f9d8fbd75f8f6b134a93116f0be051e392d" },
                { "hi-IN", "347c8130fe985bb3dab7390b2629578c5c447ce7214826fc99592bfb15740a13b8433a1c166eaadbd3a198fa225cc7fa90f1b9fc302ec6777a395a04f18acf4e" },
                { "hr", "2e6e72e5d6c5511258d555ea4e4648724361fa1faddb62f995718923d7ded915a42856264988b7c50028e93753ce3eb18cec789015b74e7e1f5b2c7c0a998cb7" },
                { "hsb", "16cf3e516690655992b3bc347327d7071f88ab5803da6121f894c90b6480f8cba7652d22dcea417c0c061a9d77670e2d47163c4645a38c7203258a7ac8f32692" },
                { "hu", "deeff11ccc6b1235e8084bd5f0422857485891adee8100349143c0b3e0baa6dd196369ce37149cf9160ae140c89539996e9cc01f972602f9ef01288d00924289" },
                { "hy-AM", "4ba0c00b865c43c94479cceb4679bd50448a183e42a6cef01e9788e8f63e74f2952815d7013e8cf0700da7d7e5218f220ce888187788676d457f0986230508b0" },
                { "ia", "028ef47fab183c403d9e2da9e83d8a76b6174120b687beafdb3bdbe4613b832ec5d3d1a21d111ea1bbb7e7395b6b1bfe543c37be048e3a8decf32116ba58babd" },
                { "id", "6814a1824a743cd38b7d6a94d746e6cc7043bf3d19ac8fda36b37ccd8307047f23a99758201b92ac72cca9b90270ca864ef3ae9f4ae76cdaf87e5176ccdae0db" },
                { "is", "be8230b04e6fe1bb05cceebd1edcdda43613756de4427325e329cb1d013b1a16489c682fff15abc480ebf9306130b0576b96b0f4e9a8ef40378a9d8256510b24" },
                { "it", "aa42bedc24c2ea5763d59d0bc825fbd79896defd7ae6bb1ae098020be8e4ee8910fc74f1ead0adbeaa8182ce335bc8cb0d39b5abe0b954ef22d45cc578f42c08" },
                { "ja", "273b7be81217a5faa7e081f4a0307b7144829a063a1e998fc4661936b7b31e71bc7eaf0786d5a6e28d57a1a4ff33cb9dc0859c53c50dcbfb52b021bfe37b3cef" },
                { "ka", "5dc9bc723bb0a79e15bcf3d8af529fa2b9d596bbff4f36b8d5972a27b0a0bb950fbd54f1f599945358a744ce178a09859ba71f5903edfe62493c205ef4f8d281" },
                { "kab", "be082f0cb1b20d35c2496d9347d66ac266739c22fcf377a62c98dfc26cf4e4454c861bf56bf24b4f5daec0035a2e80a4c883d38276c20c890cacfb24e0e1ee24" },
                { "kk", "2443767eda8a0c8e33fbb3184584636eba0a2019f64abaf869697de69c648d2edb122b08f0e027a2a73f8c2c4ce04c4e823326d6d97d9aadda9e36d39ba578a6" },
                { "km", "c33811636f3b10fa0779420d42b062a95c61bd659ce4e553cdd768b2d175a5ba091fdcbf3b6fc8fe6f923c3a53fc9c2e63991d9be2d88bb9ee86d5eb183f370e" },
                { "kn", "2c2af5f6752c90a83a54765610aecbd817d4db4a461c45f5606284b68102b1a9e286ced0edd79817748961384f13dca887b2bb316fce81de54afc844f5591d91" },
                { "ko", "da1c539aa74f7a29af2e364875aa38820e56fdea12a33c3285a9ac6d426886e6eb700fe900b7e5aedc31de27caa483eac139aa63145bbb00b8a0ff1912653e73" },
                { "lij", "b6ce51af0bced7e75a8ee7f70a14814768e33c64c14798d998bd173370a43bfdaf2527c56cb25713a47b2785b75007c4847f672242a7f4b34de5842b7bc42cfc" },
                { "lt", "a8bf43f70dd673239ceec48acb043d28119e99e7cfc0c1ff30a9accae170e365c373a870913fc793a2b2ac25b347193180368fd45f0abb286fe6aeb7f4b41ea3" },
                { "lv", "63613e85f3cdfa0f3b759fd6ca77be5fe6da074596a6f7758204b1dd948501d72212f88ba57fc7ce042ebd198585e60a51730bf1fd94354dfef450a196db9c3c" },
                { "mk", "600da10cbc968ccb25089d7f5dc7553593fb9922c052ccf89ac24cf5c5da9b1ea64a93b2f7a7d01fb8969b13c8e1b26826d5b5ab4f16cd9fa917cf9d3d5e0a01" },
                { "mr", "5dcf358818f6a84196abb7dd62a4b0b990b3f64a379239af77615d6dc05659cab8e3db57b28a8c41ec481fb9d521fc430d6171214601732ae4c6ff11d908e9c6" },
                { "ms", "62439221bc4fd9510ab720625133110edda5b6a52b2769f7aeaa99949e61766c46860bff168895c9360d7802226c8175dac82c635c2a5c1871e1a5821c0bd662" },
                { "my", "d22ae6302739e74255ad2e3188f009fcd3c1f8a50f0361ddefdb32d0c68cee407facd5e26c94afa6e457cec754bf3b4c83b89c4d1ac21f6c5082c16eafe5f249" },
                { "nb-NO", "a95b7d7088331bc802828448f5e825bcc9f80e146f1dd982719e13af718c69e08ba802f87ad177a6d64ab6f71c539a6741a04966e6e805be7261b60bae724429" },
                { "ne-NP", "5a5551272fb0372c2d1eaeaf6040a635c88a1713eaf5a705a73d514acd7c7a07e47b4e7de778da00d4d4d90d4a29304c9626c9cda388d8aa0e5b5a7d948c5270" },
                { "nl", "61e5e6235aef04e25a717c6e5f9afaa172e656b6106173c5387c49fe7ac347343f721ae3c016792c2e0b2b19861912bf9db583af04dfb281181e5c1b3514c105" },
                { "nn-NO", "58e03228d0290c171dc46cfb19fd91515405ca726997de8656e556fb3d7abcc751b14ebf3d2bc29b2f37936626023d02bea1ade1524f6e2adc10002f9bace18b" },
                { "oc", "7d75081f99c5d43a6df77bba68ae41f59cb14b6ec9664b1d15b5a23815fa0daedde6e11ea55605c3b4fc5fc82bb5f531ad5d33ac567aa979183a8d25bd1a1a79" },
                { "pa-IN", "9f7802f2ef338674af11838eda80b280cae44072d11aad727ee2e179985045d956b637d1a9a68357fe0975d2e8d526081b4e3ed3ac695db9f6f9bbf5e5297be1" },
                { "pl", "0695fa38f0544e34b3f609969edada8c02b51394b09435d420bea7c31c9a9baaa5b8a9eb165d394be0d406d35da481cd914a806965f3875be4b53691ae8e71c9" },
                { "pt-BR", "3641235a0ed6c2d5c026bc75eaf49fc1c5c582e22d82f745e7ba206a6383dd1676b1c0688aae90fbce8b53073994bef749ddea48368bb42f4762153cef451a39" },
                { "pt-PT", "45d5b6acf8c2de6ca63a51dd8a8d58be6980526942f8f389a4ec1743eb048a15e697131dd8ef237c1d992c4850252d1bf601f0075fa7e0a87f2f364350fc3bae" },
                { "rm", "b15ad855df39f5d77ac7e7b40e7f329a1d41eb07d0c933ad726c112c5d1322b7e1f6b99a33075d4596c200d02a665ff951c63765b99c7dba575d50fec4f117b5" },
                { "ro", "f12c27d07f03c962981caae1d3af112f0ed4d6c8b4452980fc9a92ee8046d3778699f5baa01864b47d41cd96a3f702f82a9013286a57c5df1b8bbe0a65cdaaa4" },
                { "ru", "08cb90373b3ca1e2a1c2c9280fd59fcceab5e518778690859e0a0b4475f73a534440f90298ae11a197f13f6263e6ee66aa98ef3d7d58065609f9558219703e77" },
                { "sco", "340717a6474f55049314dd15e35120120849d81e91b8dbf355646d21c6780c67d77ee46730d1e3aa27f6fd2e0083bd717df68077d5bf90970287d1f6290542c1" },
                { "si", "e73be0c05d754cfef33b3cca80f93625741400b50e7c7157583be7235b1df79b28de590a7380c7cc3d953977bcab19edd972063e5efef56dd2240546a1fea60c" },
                { "sk", "1182fd3ec0383fd8c3b9f8845b6b825e34af6347bed3dce447d4daec048a543777a872c49d08af270b3565e6a52c05d88e9ff4686e38098e307c7032b8a115a0" },
                { "sl", "28f4208bcd2c9e0ffb227c99b20184ad9facf0119a895b6fdeeaf2a68bee863f85cc70575f384abf2da25d91f1302fc48e7b80f0b757943b3e75b10782c41d2b" },
                { "son", "3e874794793e273c409ea0d7e1b7e6212745055c2ff55f8d6168ba1d842e784ca11ffab68a0164bf0ec3ef3e55d5aa2f9060bd9e607f52bfc77b4f4a78dbe9db" },
                { "sq", "52f763dc0e5ea124d2f5e9c89ec403beed56892ddec57b1b07eafafb7a323d21244f5a16a1e6fbb6b8df3285d73fbd39a19a3276b3494f0e53f3f0ba26c5ef7d" },
                { "sr", "67ee6c7fc11ae297fdaec72d0713f992e69a6071c67342d765c175f92ad0eaa892f1e6cc5772caa6272a0e9772a3546ba88474c7237054e55e20ed824dcec004" },
                { "sv-SE", "12e1581bea32dbe1a28fec8ca0895cc1408cffb927efb22c58ace4ba83e41ac3fe944bd1c86d5be69b7b3aa01fd45db2fc8ecb9218ac2a7d38abc5697033bfee" },
                { "szl", "5bec97201d6526f935d3a58eb8aea233de90f1692d136d3de35808aa9c4f0e1c80542f02da341474c1bb29835ac96091e629056ed5b6f33b39525e6b666d1ebd" },
                { "ta", "872d6e7ec7f20625266de4d2798477d3c4769ee7b3479463fb7d008d9e5bd76175123d614e7c9f190970c4c491971a8ceaedc10e2731c8311e6214dca6d5e54b" },
                { "te", "008472f43a3487ac63bf12079b5024ac4f9e0056652ff8bf6e4e8a7d7cbeedb090c53fbeacd577b01640f5ffaf6f1a59b0e85fb7c614a2c1459d483110d99eaa" },
                { "th", "12d19dae10ce1311e206c2baf5a0f1e4db5a3ac9ea06b7b51d8ea5432133d14e8a7198eaf422fa68ea84e5c70f522c9bee1e17189483b6154c35a93588bd4a91" },
                { "tl", "5fd2e8f2ba279df1b7aafa8477fa921e21b2122c181d3dc3146a1813a0fdff6020002db51e21f7f1ee70724062b6c12e8f072860aa2b64b9f13fe7399ddad82a" },
                { "tr", "498aa951a3b205e018991be0368924309ce20a1152731fc7deaeb407f0fbae746815860b3651aa13ccf421b6d38db59c391637dacd2285aaf953f8b6ffe428bd" },
                { "trs", "588860c406f0ce1cd191c91775214427d7353bddaba4fb1dd878478964a605b9a84939dd67f0bec0d3334e31fa7d8ca9f23088e1df176698e08db03f1292f9fd" },
                { "uk", "bf5b39244745a613c35568134e4d65f5f28c24a463eb05a6eda4b466ae54f03fbeacb3736dd157339989eb9e053df198277cd7844dbc6048e70a7d71d7039d15" },
                { "ur", "49191b13cb4e95a51dc66c6240a216914894194ab1a1c2bb97588c3ba8b579277bf95377ac7336b801099cdd31850868954b74898d79df8b797bd19fe7517556" },
                { "uz", "904fd7e5c0b575a457918702a0580ec620e3cb624d66a9c28fe87d6a78777c12a11b78a3dbc7b06f72e240c3b6d144ee35ca5461f371c8ed720c1e397a798480" },
                { "vi", "c845ad7214dffb3e35bd5d8a9416d09d2de2a210915398929108ae9651524749c7991908f57a46ed720bb6a139ae4da7332770dcc0bf0e6011f0cf3162b120f2" },
                { "xh", "46e9e1e560c0db691b90d29450862e26367ea2d5ed2fd6fd6de55bfb0895d56b98458965abbe1ebab0f7eb3347763ebbebd98fc5a3adb3aadf0daa106900f033" },
                { "zh-CN", "8792ac1e20cc8dddf1b48fa69d2689ec4142c5cb148751ffe6d3cba39d6637f5fdb77b2d2e6e88a72f99d977c498184c6411332188ad1727c32fbc5a6e38ea04" },
                { "zh-TW", "b8d2b9994444604e43925302e02c6f4511c338c9317666caf452b4a57ace26f5abcab57235e6381669b370d088715cf5a2baed9321434c48c3aee865aff338dc" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.5/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "2710687e179f885845ec0eb8a4f4911872988916f08fc436fa9449a7e56d7527430baf69ae1be77039ce059189d9043a05a52929b2e5a4f19772dd60e7098231" },
                { "af", "a99baad40211657377517c4a1277b130c1f7f4d204b323889ef5a0cde8bb4fd57ad2543ab80d7c340dbf8593055fc63c5c2d31ce5221c36ad1f2ce3669acae2f" },
                { "an", "755f9aa5725e8ea19b08dcb370a27fd9b05e71da069720dc1fe8217dd8179665a06b1c0d2f113cc6792ffdb477141536b0b25f7eaec2a303a14c06604eb06e3e" },
                { "ar", "a0e9e11c3e0eebd2cb570d29cad24eb0529b4068e1b0fd5c57d57bec700aa78986e55089e14bf779e3906673d5bc6a3b0bf3589437177f1fb90a8a649d0ac473" },
                { "ast", "466706fada937dc946166b01c55729d45c7dcc06eb82dbeeab19256e5b528412eab7f765c84c903a41725076c36c143874bac3fb3f7ab7f78237dea9315d11de" },
                { "az", "25a3578e5b805d1d3415347851369b680f953793239fbac083ed2300bcb1d085e0abf3820a8b7fd5599f4b3ee018be1c4af85b040cae4fd0adb07b3bd4d5b0bf" },
                { "be", "7a21f2efcff8f7171b238c057efec535be933a5f4e5ee8f3eb3bc50520518ecd5b0e8d2b30c36ef7901915ea60a2fe592b41d725c7c7dd6e30dff218698291a4" },
                { "bg", "147e4fd4a89bcba694dd7ceb70a4bd11ae22a6d4e3de3f1038d7e5f69b948c492714cba613ac247bbec1dcfb74f5ba0e43d7bac5943fef651574ada6a8b230e5" },
                { "bn", "964a7340dee3cc2f19d438a8b8fc8dff65039931fcf86e0a67895f2057847a66eafda0d0a3829330832ce71cd05302e7af520a02032bb12134bf202b44119981" },
                { "br", "18c907d1d1bed1caa82b1be05024ae152f0043865902d210bf67150184b901051797e41f468b411a9169d4fececd8dd4b541a2940abb0d45b1328a2343bd7278" },
                { "bs", "aa46ef667226014f377f6991b60a7904eae90c7c441df239c67d46fafaef0f9bd2eb4b2bd4284174f932b8aa120b87fd9bb4bab7edeb505b3ea1319fdc75c7b4" },
                { "ca", "1c719b7be7ca871949483f60aa38ef6f41d76de2283bd4bd278556a8f9308f9070fec39d490f20e16055e985d3604619c79c52725e8f7b393ed75197b47546e0" },
                { "cak", "a071447a2223a1e62de4dcb9b377698b07efd39f0606a3e1dc9f73fc990f5389143f626238651db77e8ae6ade2e3bfffe2d494bd5518fb43470b7e25bd136f94" },
                { "cs", "cb93609fc5a9dd34a4cb63048045e0f19987525609b0ebb0d02e042d6d219fa05e3c5dbe0b2fd0630dca2590d9e38dd43ca79aee061ed5ec3c86492120418fcf" },
                { "cy", "fc652301f8ecbe695a8bb02a71524c14a150366b1ddeccf37c1aa4863e9343cda5318e9d614b7a2f4eb0a2b6f0440dd61c6e77b22b8a71c0a2691d18d3e2f99a" },
                { "da", "868418901ddc0ddaacffe71bf7242e4ab56cee161789749646e28761d29901c22f24a972b6e799875d43a4b275c6befe5d78668399dc394739e7f12e23b96c06" },
                { "de", "a3e93375f875ff86321bfc381a1a57e885bc91a4320b734a7d87ce12732973a2f8545ae82082ff46752d2e16a7eb80783d272f4055a72bcb52bfb3fa07824ab5" },
                { "dsb", "2252df43df9efdbfc04823177a73eade89ba68a9f97f78acd0013c1501d1e87286fda348f33a415983e89ee7311c8f3cbe800ef0027224ee492fa451617d17eb" },
                { "el", "6026038d70060530c793a214379380dda655cade6d844ab1cc0ff24a8d5008ed31b91419271b62da72574a40b06d7983265923a912da7e2784569ff310517dbc" },
                { "en-CA", "898d0a8cf3d3608e02e3d7593f030d135347a0dc6915bf3b87306a51241c70eccd7f50b9e0b335f6bd2ec269c993e42073b973c5fb368f1f9dff89d2aba51e4b" },
                { "en-GB", "890c6c506e91418297794fac6e6f4ebdbb131a85e620211de37f51e09748ac9cc49c192dee6cfb6dfdb5c71ee9f80ef357e7f79a4257e501ab19f20117d72143" },
                { "en-US", "7a748a2651bb003b0af41b1fa49263364a4bd0db598cfbba5197190e7f352fb608c5c0b09797825a26329a5c86b1c789889a7b53c6a9af46ce24605338828d33" },
                { "eo", "27194a6745dc4c88887ab263e459ea88a748e9e9bd562d5b4e07a6bde655327e10eca4e7d84c01d3adfe587341850dd8435be172d30718fd3514c372191e41fe" },
                { "es-AR", "3b65009a61036f228df364b59405d9f9415871ba2a4ad4d0f677f425430868b84282e05d7045589eed80b087ea3eee5e0cb9498f4808cfbadb405c1b3dadeebc" },
                { "es-CL", "eaa434c1918479ce52bcf0b3a6e409d9b3be9e30007db32030f5529cd80c495599ceb25c51ed10ed95019408c5af3a2f8bfa5b40c003d500ee062d315ecf0c1d" },
                { "es-ES", "e14cb5df3c05f15e7f3e19f5148e4c5a31f4e38ed35265cbc9533996f51899091b33c441a94b7fe2675d68da21d74cc3ba7d995227118603b20e06a3bf4d482b" },
                { "es-MX", "06df1062ccd20268fcce2d58b68e0fc30f17831b1ee4ec1b25b0a558089b4b3fdb4bcd72b0b56b78bc7786381ac8a7ff3a366256fe1daafdbf491d53a3ba74f3" },
                { "et", "a6034655732ec44bb43c906682e5486b588581e8cd4cd87ffaf529a61ca4d1d652b420cb94dc0758a941b91304804f2178ddadc9546d466956351a53477748e1" },
                { "eu", "481825a88900268b700b21d3009f01b8798eaab69cec228d58b08c7b170584a66aa674f5ca64d8b51512e6a07d3baff2ad9364b783e944dbc68a8f4b92097276" },
                { "fa", "c10f9243198d1d810b6058c08e62d3b827a970f5d583d5639c7b80d2b04642a56498085d3c22197027319d56750c7472ce72c499b7f1cef81ec30713bbe71b8d" },
                { "ff", "7445664c7ae4531cf0897910c4c028b482f22d659da7aac4e9ce440a9141c7490bf1ea42de4a22adcaef9f58cefa850687197ff5cdb827c0e02dda1d0302872e" },
                { "fi", "0d95d58cf7c792e4f508d17d5c156ba20f20455c981aa40044fcca308822c9023e0e7722666a1f86e33722c73b6413c7d2dc1a7ce26f9d0dbbd87725b9029503" },
                { "fr", "f4d8357cb4bfeca28c0f007e0e090b1f50129c704986fdf6baa314dba60eb2239b98b01bb94b567548cb7a441056fd3976a5d24c2b2a408006950e4610707352" },
                { "fy-NL", "904ec374af097973e40971376b96c93f5a151127b8a034a083cf5725654457360188e31f07128e5821ffe15d49c9a8e7aec535a284dacdb8f1aff72949e7b28c" },
                { "ga-IE", "2db3a32c75f2ed5ba5956f6099fab01678ad4fb156ba654fb97759ce4d0ee1329bc5259c0535e81e5d46df1f9680650f45bd7bfdd0aedd7d0aa7316ac645d1c3" },
                { "gd", "69df5f1a3740ed1bc305f0824fa65c2e8441090fe7abad017a76fe33b44e478d2f5d6f4bc832be194b5b8ccef6f677a3613afb7bca3ef0811a7dd5c2b3004bf1" },
                { "gl", "e7972a74403c075ba0d77906fb4fec365b34d1326ef38a4a4641f2a332e1ac383372adf745619bc5673e4043e723bc7772ac6227ce48ee037e96133a4dccda85" },
                { "gn", "ccdbd0104fd791785b8367b62b6e014b52c07c79ee2c05837af7453b0f08eea3cbeec05a3622da9593cb63654d95132b032f06356d3c9a51894adef827aca6d2" },
                { "gu-IN", "d01a2d7b378d3feb45e814447aada3da3d783b28611c3e00b907817381dd30100fd309b08af2b286274aaf2cada440a73f7407d1b7fb09aac9847e1c718fbe6f" },
                { "he", "0abe3bb3d424c13a52e6796ee0257dc41620de9a3c79d25d05e0bf7a14d31f5fb175192b616d6277a1c9fe05df897cec86ed4596909ad5d9f7afce658ab531c4" },
                { "hi-IN", "e49307fe24477880c980b02561dbc9bec25645165dd4ea1350b910b32403373b12ba1cabb8d3ec13894ead1b7c20dc3844c6381a44df6ee62e660395426fdccf" },
                { "hr", "7472f8f0911d776b5a43ef2267826d6d5471cf68c3033f43ed410526431505e76a10632054feedff594b0305447cb56d20be087ceb8bc2771b94cba354375d97" },
                { "hsb", "032acd12bdd4fb6096cc97590661cdc8e25e2d9b6b2682156208e963bca31591db2e44caaacdcb32e2570e8d9b87198d4809122007cc0362f7098883b8ec0928" },
                { "hu", "8a61608b8e1514e1e8a44be95dc9e239f34ee1f0391dc79e6340f90357a78094598abbf848ea948de00a6f9a774d8c8e6dd8656176176ee081010c788e2be6f0" },
                { "hy-AM", "13625f9e6c6f426101b7943c272569be1ba98359d04cd1bc7b17c9d5eb975a2e60c57860adcfe337925cb5b6fa6f75cfac8bcb62bea649ba0dde6b7391deca33" },
                { "ia", "665e6dfdb32e2de274bab6f261af07944d1f67880c418fc2db61e44c87bbb394832c2790cf110a76cfef44ac2a322b262f68791f31bf7ef41fdb6e9351b5f668" },
                { "id", "bd36b3d3704cf8618d2d9ba90144b159e93638b72f059f1a4e422f3856f17183a35d154270cbbc59b4ff45cc60ab261fa884f0067884320014d69aa756e581cf" },
                { "is", "74ec285c541382788fb0c5fd2030a6b083c5abf0779de2571d982df1f6ca9d65113c537e2ba67f97d6f4587cd856f7e37f4742e2f922775626fa9442c5ab795c" },
                { "it", "8c3a49ec2185f94dd9d180ae6dbac8d1b96b36897f286ba9127d4a747d5b95a3bc483f746ddfeddf0a83f67a08b7d7cb9862a0223302178720a3fab16c9524a2" },
                { "ja", "051d554a11ad0a25f359712ef69d4209da9cf9fa41be3c449028096f2cacd43fd7bb640832628762a030a9632e249406219a1f81081353bbf20843a9ba11334a" },
                { "ka", "b5e0c1ff633794d35a66c746c6daec1658f8c12eebe159c5eef720b4846f79f15490fa7084ff4f24ce51306a6ec76dec50ac05af6d9168759630365d63647990" },
                { "kab", "72027c90ab0cda0506f12ed5d2fbedc4334798370d3543f3cad726f06cad87f261517d832bfdf7a23580ed64096a51a4496cc734a4a6d46ccece8696e0b80985" },
                { "kk", "fa537e672a4a2155b1d47174c64eb88271fadb6119feccf848ce17413f85bcdd65bb8d8b22dd1ac1d1efb646bcd8d4766122973d634112c24f3a06ab15ba489c" },
                { "km", "3b4aba32446fca7723e797a1eee9f016b5f5eead2d38025cffc47bc48f2ef548743926f161b5ab75b29adc13063cd9182e63964d3d37ca8fc1870290d1a3434f" },
                { "kn", "13d403626f1350e50c12ede30fd401940975986ace71565d3791843dad53ec7b3883d1c106c663de1f7a981bbc239acdbae05a2c6e59e2eab44e501017dd3704" },
                { "ko", "033c5bf1b3d4cf9e2e8309e52f38ad6f6cebcf35d089b77c9a7e7dead8b166c62abcfa93639c9863d0c9ee1c2226c37c4309032857ec0bd0f7657c46c7341f58" },
                { "lij", "abe394be7e1a4b8c33d2df2cec7eca3f82ad5e3cda1d90432ae766de7311f9a256089961d3e750ba748ec071d903a0c1b6926bdae3bb385e7a33c6d0836bc132" },
                { "lt", "a30be02b0c42173c1188729d1511806cbf3a591a900cb3828a068a0e5a30817196425307929ffd0839b58bfa7cd9d7f7311159b87698c4c68ee05f8193968ef3" },
                { "lv", "b39cfce297a76a5b04a8464a1c4a0fda5ef285c151fec3b7d9ff4d35dfe044aad12a6192fc450dbe5d7cfb8f367030935759be6afbd3ce150ebe4ef500bccb39" },
                { "mk", "859727aa7e94e5607a6a8a52bd5b57b36b2f0941ab4656c16c0a6986530f81fbdb6d77990d231d6b03dcfcab9c264a97473a97d7c6d4abd0b462034660d86368" },
                { "mr", "f60fa817b596670514707be12805da9a10ae1b298f2b43a3dfb29ada169a99de3c6a068bf5e24fa56580cd2aee52962acf01ce1e63d113c212c8c79ba0f022f0" },
                { "ms", "399b11fa7c218c1e4edd2b9dd236ad5c8918fa677def63b80501f6153d91ac9cc6dfb87a237f2bda0ceae21c9ae39d5e34bf7ddafac2ae509b87628898e6c059" },
                { "my", "017ed4edf6487a536648a1fab7aa12dde17e431b773df9eed7fa17e7e9262e3c835c21f762c946ffccc77c05e3e6904dd8fb615e1d366c018e109962126fbfff" },
                { "nb-NO", "51a6ef61c1e2de65050593746f81d24d88a2b0b7cb2dc9ff177bcaba4cc4f2b79beaf7536b374ae77722dce020e9e7a8a215fa3e7967216a0b036d81111e8423" },
                { "ne-NP", "1b1addde36d45d0ff0638b635b8679c4c3b345e64041a4d8a3c9b4a9304f67112f8489220825683f28ce9d08bcadf696f08847e7c4d6616cd0f10af353f9235e" },
                { "nl", "b0666043ddabfb673c3f8cd88f599bd21e13839e706690366f86c1e1dc4e72c6c5c023a2e1dff6cf4bb9880b6af9cfdc0c43e933c1ebb19d1a0a75db330dd2f0" },
                { "nn-NO", "7242c9fb8a3a176ba5716c13dec5ba1d332194f85e4e0d8c3afe34d278d4c1a541b6568c4e0506dc9672d1738ef8471a228c9b507480277203ddfb6c988287c0" },
                { "oc", "6cda548458fa8f5009b268db1b05585c7f77fb6e6f7d8e499e60dd11081cb42591e32abb872c4504ba1f2d0749f4494b2d587aee9bb16cd77c039a00fd30c62e" },
                { "pa-IN", "54ed91178dafc3e4f9ab27386f524c4456ce4a0d0580e5047b0aa0c53193b2a6329123c59547ecfa3a98580c51e5334ceea2bba74210eca82426f01359001d55" },
                { "pl", "9b21b08df5015f097a8d22f279ce33a5e0dbac39c3ffaec462800d2ba17fdd8544138fffebedb866859af74852b812e4b3361c6fe96880e195e6955779bc6eb4" },
                { "pt-BR", "6f50d3fdeb44ca49dd816116c39c74892291afab040daf7420686976c4313405c898698dfd3d6d848b9f6c8168f0298b03a37ca36d9310dd3d7475866ebde114" },
                { "pt-PT", "89201df22049f19f539ae66cd07eefc9307a8f141ed6d0a48e000ce997fb2f031c46f86b7d9aa06dcb03f05254785daf26d9df0dd130b50fbc0ea542b1591cf2" },
                { "rm", "83860ceae511e519ced46c71a275f11ce4f3b2d426fcec1a28c7f63d7f1e1cd7a4baea932400777d414e22663e17612685ed47b5560e4c77da47903e9a5b53d1" },
                { "ro", "7761ed1049f03c1aa05caf88ecc69c834c8b261000f25952d88e0ae6f008fbc72b401fa764fe91ad9161026cc68db688e0f4798f47b355e968784fd743a324b3" },
                { "ru", "c78c486c44718298770feb2ced816834b8a347982e877d862118c74e3774cd6677a3a7a7c7ae6ee5e8a053ab2e625bff1e14e029f8c7f7f09175448f6ad6efef" },
                { "sco", "a03481f8c8356a5528516a02eeb9d43ae6ea5442a08362db5dae659214c4b5a25691e61304a11334407cc1d9a73daf568f90d2912fd9b8b285dabac1e5a2125b" },
                { "si", "c96bcd7a3e030d53d1f407db5c27fbdeddeda3eef040603e57e39b7b5441e5c50dc9386e105dac351622bdd419db692b94b9cc040c3eba85f3ecb1cc1fa79160" },
                { "sk", "ecb16e7562f8a855f318980b8b19b05691b2492b73fd7e0abda779d7ddfb06b18fba97029f72286077e30194b38f4625a804540d62ad63551e08924f5263c973" },
                { "sl", "f9df728c4abd1f2bbd4d9e05dbb7840a0b226d8080c694d74d3e76cb725626ab1e98831e06ac33d5641b6f2df95c0157ce9a0b23fb01d870ef65473385cad23b" },
                { "son", "3e006e85b4ded60d4fc2a6be52f1d79fa317c6856157bd701f381d2ed14a70ea3d94314a06351dce636f19e621782b7c1f44b0aba2b778dd6ba246f019359d3b" },
                { "sq", "c72f14d98d0c5f9a3794d10055214230dbc7470ececb7f3f08115d5c77bef89a2a8acdb54460ef3f6b684756d4f8ca8eea66e6b0e3066b3de345fb59f5189f78" },
                { "sr", "ba4c97b24776371c77fa6ce17ab04309545ac2bbe18448580c462360ae7c1306601d538287fe263ada81faf4b71e76a410b55af1e55db7fad88e9e736ed97184" },
                { "sv-SE", "0ec917c57571aed42dafa44b6530b269985d0c17aa03b2921a60b20f77b086ee77b7b4e354be57327d90372444af706be936782c4012235637a93cf1661bda51" },
                { "szl", "f7c8fd9b48485ef4084dd2c59ec6e7cd9777a07790879de136f208e0306bd48190e9fa70b5f34bf13fee26ad3f648bb074316947e61a6bed90b3e74d45cdbefb" },
                { "ta", "fc0a158696eab2b4f3e2e195141d4d3d8e72764735769320b1883dac6c718c088cf52904301571361de25dad274af5feeff66c0b0c4e3b53896e7f7a4b0365c2" },
                { "te", "8e116917f043440428f0f703ea19a2421bac2a90e310f6e0deb98c7c7948aedeef774307be41b51348f4c7d5f1a9eb9a3cfaa9323afccf5640ab498078fd525a" },
                { "th", "c18966277e43aa44aaf0201be250a47450dae90d2daf71e851123a7aa0c375f239ba8704c5a0f91962341a642d5d827818d777c86131708e00aec4920cf1d812" },
                { "tl", "853ce0934d45d050632d8f040cd7620122cb728f863e4ab7a932a17effeddc64b6a0365a8238f641d1870306aa89d3ca39a3789df60ddc5e5818d40e32161ee4" },
                { "tr", "5f1e46bdd341a5eb1b0e91fdbbf36ac6734154aa0616b469066f523748e1d8d29b6b3f6e5a1ec71ab534b734ee2f3fcf78547ae76c2e736f569a3189ba013a0d" },
                { "trs", "22f4f6169d38fcb6450acc7aba5c0332ab78d099fccfc12364918f6c32af3e4d5825184d970adea1380e2083bdf70d2103494790426fbdf27d1ca3d28377058c" },
                { "uk", "be885930054a64e8be249440f4ee6ec8114829c123647485b502756f8460b49c3cd6386a9fa7fcbf5398b804bb31330258a6ce7bf82de178129bcca47bc0be91" },
                { "ur", "499d1fe672baf890b4bcb0c8cd883bf8c8d4dfcf1413090878916d9046febc7a0e7e401427a1905b30f440e7c0729c64804dda7eca079c9efde3be93f6aeab6e" },
                { "uz", "e7f49a137a51f4e2a5bbefe0d26e278942cb24899dd0a5c4a2f6ce9841d975feb923f602dfedf0bd1a9f1454de94d8c6189114598c672eda252a7d5ee31f86d0" },
                { "vi", "8fe8f5952f59ed9fedda647eb007becbb55f46c5f65c1a5c103b769fb8b1eb40d0594e3d2d37ef1c8db9fb4a094f2d5d3997246f60db96e9f3ad6d184c98ace8" },
                { "xh", "1ce88b956e1aae5ea5e927439f19605b772d66648afd37b52870f928bbf190c3bb6e1bba71574594e63da1ead4325b336d3e91068fa65be54fbc75b095a66a0d" },
                { "zh-CN", "953bfe5641ef9b0311f3e8ddc0a136abc7a98bf30f638883bdf09a2a631d08845694659bec56de056b08be184aa08983d3ce12dc7e77b4d5012f37003b2e4b74" },
                { "zh-TW", "9f038ddde131d3c0b644390de0f5e51790a7318924bbe89def061bc205485913756d3fa67d3f8efd2158d37ff88cbe16e2e87a2042f605314f144027c1c4c251" }
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
            const string knownVersion = "106.0.5";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

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
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
