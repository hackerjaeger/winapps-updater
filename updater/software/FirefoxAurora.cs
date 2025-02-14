﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "136.0b6";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "cd144df76ddea117c0c7cfae1a082993d6a991ff4cf87b69617622088f099a16425179ffa45dc579636c31724c06ba7158ef4cadea498879306a087aa37bf3ff" },
                { "af", "7b0f9fe8ec0356fadc58561a24dcf47653413e913ab9900c830d3755e0f90c1222db9c79f398059cd128555c8203049fc235bfc267ef56c28d95c1988a867c20" },
                { "an", "9f520a675e7973757b01e7793edb73f4ceba62a0288fb64390ec9d9bbe6301db3ea849f90141ad3eff9254a74e6f3d2e219db9881c4c210a7e93ad8e1e318368" },
                { "ar", "286bee3b91a3711e71b276aed2540eb5c18554d9dcb45c34494dfbc5c0be1f0348e230aadf51e814e42c638a943b2d8307b71cc4bead40fa6caf7346e00a73d4" },
                { "ast", "51443250f3b8f3707d21e78f6c49ed28a0c51cf30a3eb0859ce977edb26300d844b7d2ec5473649254490ab3d350a67b60090c55684f7f4134dc8c205f923f5a" },
                { "az", "a83b3ebcb24618b11ea40da482f91979fd908b74134f067ee9bcea2b4512220ae0a6b80aa85cd85583add12f7863a4db0e4e6da0042ac921021aec0c44e6764a" },
                { "be", "d10bf9e0b03e5e92825a983f71be9216abd028c37b174824fcda1c46a940d6e7fd3e07908444e74dc2baca125dfc3295c460dc4a5405a8fcda3da5d23c0357bc" },
                { "bg", "caec7c40ba17e26f7edf36f4cb7a204e197088c12f753ff0deac6d8b08f1732df53ac45765d7391bd06e5f9c0b4b939c5e8fb659ae965afd0f0b284246ff0b4b" },
                { "bn", "968d2c6baeec0b8813e4c65a4bb2d314ad187636afb36b2501dd8f87e24996dc6a982f23b73ca72dbd3a6211b12d40f1004d973b2bb3af40f722a0f0ff02f1e3" },
                { "br", "1d90ed4049ab15d599dcf8d1aac898ff78c9e19f2b140713fdd4c70fd40d46936b524c1f60959790deb666ce0e4adde15fb284ea264326be9e53ecf4bd0dee2d" },
                { "bs", "0d297831a0af33bdf572129f551ca946929f433102e53cb20aaffdf542be717a6c85ee00adbc9cac2d01d22a74bed102491dd4f5a6564e22640e08f90eba0c05" },
                { "ca", "d6d9e2e4d4b8b5d8a0c9293704ef3d51db4f0bf41ae906087022f16d333dfec689285c056edbf8a72f5ff410da021a10c654cde8f920974bbb0b1dccaf08076e" },
                { "cak", "257219c249e0124c328e038e745052776df99886a1b3e6c422387115e102b2e8f05e2977ed0ba1fae3f5c9d351bc6ab99905eb41b9547613d8de68f16bdbbe20" },
                { "cs", "cc1be29891e093d657251a9f24d322e38e329db4a1479004438abd9a50e49c340d98da7d42c50f964fbb5fa267caf3f8dd7a55c86d0fa0674ec90fb6f0802e53" },
                { "cy", "e472d51d4cd82ee5e5030b55aa5d0058302b4e41b9f94e0d41202a33033342b83f1068398262c0a9a84faf2535a0efd3451d46c12eb02e095fe4cc9062d1a896" },
                { "da", "b678afc5415e4cb722dbc5399a5a878509cb6e80afefd9ef406322fec7b5203396a4e7122475066fc80d5caa6cac4bd167bbd5697ee567744685b432a78dc0e6" },
                { "de", "923a7af9053fdbc91ddff0d35b11532a534495207a4a6f73880643ce49e9c21a058eacac71d78e510eb54b94ad004ddbd45c4046a7bbb6b88529f6dbca813b5d" },
                { "dsb", "1f2588609e191bad0f86cf861ebbc96efcbdcf5b0bb753c4707590eb5b4377339cd5bca59285260781c17dc108dfae538fbc5b6f790a0d6519f9cc1a29b9345c" },
                { "el", "4b510451cf20fc8ada9c0ef19cf8f5902708b234ad5df7f2a1911b1a4a848fa4e40b14e4f2a145d3ac8fe295eea56a465029782e93be5ee5d7835e5e001732ba" },
                { "en-CA", "2c523a0faea5fd7f1e3062e4a2d76d6ac938908f87f12e5e628301a1c3b10051d5cc92043c338b18abee34f7949685f809c6567f5fc01d6d0b6fe329afbf865a" },
                { "en-GB", "77b8f4afb660cb2c62668d46b2bb2fa45d9493e766e169f80ec33da84205cf00d15370b8236757561887b6f53ed78a940def4fdeff643ad09e395d3386a39d48" },
                { "en-US", "20c07030b106951d6fa583ef8a650ed62f13e12e3325641edf7a72df6f21b1ff0b69c3d1a76750641a2f2028b0f4f990a98a63dcf2a643d745ae790c130c4e6f" },
                { "eo", "77d1934fa7b6191ed6fda8defa900af8011d84589a1cf516a438d361b40f6ebded96a3ee4a53f1dcf3dbb697c469a71601faa335f794a2c5ae1dcc0c9abce1c3" },
                { "es-AR", "7d60884ad416a098f1a59ca64c934e37c459d86a82be69ad5df039f63cc0060f4a4b3b8181f68d782597d7cbfd5d68d8a5beacacce60b1fabbb5a4a1fd19a9f4" },
                { "es-CL", "0d52aa8c5eaf7e3e0a0b73fe642332315fa3fc65e573c8e0b81d7f0e1b503813af0228bfb71dcb3e59d146f74d9e25cc8fa0c524e08b51f7a04c2eee32836cb3" },
                { "es-ES", "5f3a3de870d219c3fdde7cb1ab538d014d6c41a1893f9682ae7f3a1a1e95b8d1ddf70856a78162a80a18d24b929423dfd3bf15ec734502c2117159e1b27d3e68" },
                { "es-MX", "c44ebaccb61f2e6b53505b00d63ceafe1a77c21f26e6713cbc61da75bee9531c77f4dbe7152f1064f3732772aa37d6fd9bca5a2e28c42cf353e1f444d9c7f027" },
                { "et", "da062f37152a55ccdceb15f80f740b5d1e18c90acf10c72bea94d0498f01c0e5123f8d6217beb431faa718bb7a2c3fe151481f6caf3c2c8617abcf07c8bbfb48" },
                { "eu", "d1a33d35d6ee5aa1a1ddcaecfdbda71af446535caad1854354cd9f8215eafc44f2fd6b75282ff3eef3736737268b41368722f2a27e4970043386d8a48a50fb38" },
                { "fa", "99b8f4acfe3aef46260ff48684552bd6e794542c86f7c8a551c1fa35fdd924823159fa4895f2a4082fca7fe6e3570173c21a9139613d55985b032060ee26aa8a" },
                { "ff", "3f17b25f82f4ad68a95910b2fe7e26b54149b317a9d93430cdce80364bc84345aeeb14db5cc0bd70369e147a75bbccb621def1c313526d8e7e6af4f1eb7def6e" },
                { "fi", "2dc697c5103568b55bdab4d8450b47ff3f4dbcd0144bc6d1d331acf15574a538602169db1dc651aac001b4285cd72c133137ddbf4c8342fb8b44d032deec5779" },
                { "fr", "71d8a2d74bf9497151086c53e06d837cb1d7a81656498d70130b4179f04043bde179e263739c3e3eada8ab10deb06796f36c309d7a8e1d65f316dc241576c583" },
                { "fur", "91729c1e786e795e023c170e5f63ccd39f134ce3f9d094f5026dc9391a5e56e6cb5dae443a74c77e47c93db597fb34abe58a0c2fb86cdde02acca4264e95abad" },
                { "fy-NL", "4c92ae7d3c9e89df7cbd86a8121809f4d6bf69cc2b7f0748048a8c52dade8ec7dd1f55fa174d0461daaf0fafec1cdc160406a64c66a482c5e12beeebc2adb9e3" },
                { "ga-IE", "4a47220caa8f256b8b27e38742b055b3c2f9546dfe919c6df9ad91671c83e27c2c617d2de6e361d80a261ca07560bc1c69543f6f30f5fee6c227f3f02f74146f" },
                { "gd", "5c44811c24c654e24e0df6fbde6041cc02b4e86326e8f03cc912063099911a571b8ebd6658c4e38ed2e26dfd0f9359aa42072c4db446c629c44da2f89d4510b6" },
                { "gl", "83499e62874866b0a5f5cf911527cc792d027204713d611255822abb0b0809d83f151b91a3172e16a3ba449bdc7faba9e5b5729560272a394017e4fbe89b40b6" },
                { "gn", "9a7cc96042b9f86b8a0b1c1b7353d8c33105c5ba6d3b663cfb2410c9bbc617a47061b7fc4e7fa945715a9750f0bae5500af08d28d8da5e39aacbad24c0354207" },
                { "gu-IN", "e63cfd135cb336af06baf3c2a5da2c7a2d3c2d4a0f2a319e6b6e72090f8caa0cf2e94f9a8e8378a5b8ea33cd5e06b3f8f44a2d05ca8266cdee59c4f2013a30e7" },
                { "he", "7ecdbfd99e8961e1fc23b4b3f09224c18ebdfaa6b58deb928cf362849abf70145a21ee46780d470e381a4337bf42f3da998b7af9a1da58e10ad5d86a84c7896b" },
                { "hi-IN", "fc7d53a8b735728a3d2e7d0e8c00b3d0a4863d0d7eb4423c296e740feaa03d8e25688da018ea0feb221097a1e6bf8a1473728d3e3d5a2b3fcf8c7c617e305f13" },
                { "hr", "b7cc2c49b61f6f26acc533739614edc9c130cc5e4058ea96ed181ff949b3ee538b94a3a3946dfe626a3b8a4957ba2ce249939e61a2d681e7dc1ced05ae0bde30" },
                { "hsb", "0fcf66843d3cb6eb57696aaa08abbdc58fa58bf598ec9f8912afe2d5ea6501d9f5ee224b80672c2b90e2153d4b72d5c76552875a8b6512721d43ebc924044719" },
                { "hu", "98703b3ff9a22b682ca1e51389cd55f4ad2847a7f45267bd5ec0987ca4379b444317b4e4cd2dc583bce58311efc2895521901fd7bd8f162d1780426140fb7c85" },
                { "hy-AM", "fc562378268f6b59149e2bde1b89b5b0f5894fdf4af9ae16fd517fdbd3aff4d4ee4a935298ae39d07bd47366877b077675baabea7297b465318dc77666966faa" },
                { "ia", "e3f2ada39cb59682a452e63ba773ba3b28281afd22ded87492a498482d5166bf2b0d7f254a30f00eed705af34b3aab7e179e35ac802728c7c61f883c38abc368" },
                { "id", "4f7cbea29c4a87fef77c751dce8c78e0f836c36d9f31f996572427b703111b4a667b1d4570f630cf03405a266a55a26d9ed901868885810280cd2837f03fea40" },
                { "is", "1257afedb85e8ac898b2d768905389aec2d7dad168c3b76f12fc035a55d5f8cfbda52e153df2980f40a6332bbbe2e7e942afb49aa38c36dbfbe2e4e78473ef13" },
                { "it", "9c6cb0ea94c6b7ed51d128709388545a9efa3ec66da4e2c0bd59e95a7230866d8ff255b4053271b7232b0b94a81618595128891d624a75026803864260b911d6" },
                { "ja", "f846426872dcd975ec5b49b1ba29dd7da8f420eb9604c967675c419bf7fd3540c382213673d8ebbb791dbf828175d8982d4ad3722647444b598222630ecee0bd" },
                { "ka", "d50ef3510fd1d99f743e74876865a7a91cad9b83a61df71e886c578de27109904e375b2e504c10925378a24c7a42e2be7000034e32bb523da02e46155770480f" },
                { "kab", "ba2fe35fa24b7762660b1728323af8eda3dd32993152797b0b0c0bc07d80e8553f399aacb20e58eda3218f7ec9b94bcc63c6a11bf5ef72fc8f19ca0de43fba81" },
                { "kk", "a7b5ff92f4449d1ea997cc6f5cfb80d14c04b9615b1c780e7114678f426fdb1e1c27368a737b52a259953338048e7ef64d8cfa44202543a5ea152d0b120df66a" },
                { "km", "49b75362c252bf9f06d8ed763be189d9753701eed0ceddd14861c8ed19dce64ed54d9a4f63fc2dafac395ad6837fe53db06f6e4b667e46fb58e9beb7d7f49a02" },
                { "kn", "ccf23002a86bd52ebc3550a7dc66698b9b6fbd012f34e09717b4955dbd147f35cea9a0e92210c3f0422951bfce5162376d7fb8b9a4ab39ae88441bc88ffca99f" },
                { "ko", "18c095e29b85cbe2eab60a29d152ec366cb7466abb1428cbc1641e56e2ac10157af65c0a5c579cba2843358e70cf784526d33c5c889107fce0a1ebd549812c99" },
                { "lij", "6ffc402a3c82304387eea4e7f33a965dd365877dd75cdfa5d218696c091403d98d85deee4b6e9bf59946a490916f574a654e6855494dd3d793062ac467649305" },
                { "lt", "7158365cd27f8af456d95b67b780117500be4f2f03a52ebdd758e67f97cc2aca28912be917a429071bbdb1b75fb969595e2a3cbc330e551eadc6b050d4847bf6" },
                { "lv", "55c034afa7cf0875667b1ee8628ad4520edcad3390c608b530025e56b41bd256a1b14695e9c655a9548ec58bf60b4a00a47fd4a450c956be1bf5409f5be02a45" },
                { "mk", "13d86d63b7ed8ea555e1ef9493cfabaf952540553260d4c4c5a298fa589a218fe308df145e3462e16ff85a6081a40b8616e51ae112c8ce2ce37d127d2c6041c5" },
                { "mr", "ee6639977d67787642b6920adb067c13a068dd8c164a0ebcd50203374c23b27392cda66b17dea5483ae4aee3ef515dc7e5e455229d6f1fe10d8c57c6e3f3ee20" },
                { "ms", "6ab54248aecf78dc3d117cf09c2e620ab5ad07db7dc9fd7bfb71e94ec02d61eebb94cdb297bb5d9c9952d92fd69c91f8dde3081fd1a36b24984bced595ae1672" },
                { "my", "6c39987e749a62da8ab4f58af0869c28c6fa63097cb3ef6021aec5ccdde1583155f29fffb6305d2a3e5d4e1a18c7d6a3be42e93ad9ce85844a87ee5741420c21" },
                { "nb-NO", "2962e68b90ecaacca1f8fdb1f849857dfb235e074255cc0b64794b1d0a049efe55c819e26d4b04776cf5d1a510a01e737c84d424b43c4fc73225aca49129aea6" },
                { "ne-NP", "d37b0bd66e7375a1fbd54532104d430d1c1245383b1aa35a1adf2d83ee20afd885b9e9dddb7dcaf11cf0d71c8463c9e6a97a4fd30a22b0a3b4443dfc9b22f01a" },
                { "nl", "67418c0e84a98fa9f604c5f5abd13f81563445a7ff300ecdf11e48c3a6bb57b577d84b6a226a8fe5a12bac2129f8b30067882421fdd4e70d611836de4ad2ddab" },
                { "nn-NO", "23b6bb9b950ce8b6691dfb23ad5409ef3adfd763f72c3a6993d011537902401401a2bff2b4e3278ad5317a649cf5d99d4133b5010633f9f3a88c2fedf3ad291c" },
                { "oc", "8bbcbab5ca8da20556c116a1fdc10cdbe73ead51cbf9f1aa28592a3b8555344045c4bd552c03af759c3bcc58b39134cce36a683dfee5b38873fb2b9f88c3dbe6" },
                { "pa-IN", "0e6ed18b16caca64dc41377ee6eb227087d7a00fd9d38fc3f1405998560ad053aac5d961b4f2df4d547202dede564d2c714038604e0c6d1e3d53e26c103c2c9f" },
                { "pl", "25c82c896e0725bf102ae53cd10d375963da4fa98ac70501987b5344080694621d44e8b3da5c37e7fa16d1043859d2e3baa2f41436851f3f625ef0b7fa115dcb" },
                { "pt-BR", "208e68fe16ddf64c5f4c2a9bbae8c6dfce3035d00eede733b1a2fd2c69077885fed2f894814da66889af91258d45d438bce4f6941ab9d2e00e4a1d27651afb5b" },
                { "pt-PT", "3d3786fb6890dbe1dfefa9fc61bf611dcc5d06fd258d2debb2da396be7926ac553fe67a59a15bf945085913ce7d5bd4fa1e065bef6a263b6cae800a8e4808261" },
                { "rm", "c828c3accabbdab22cf9b6d6d0a7a735896b14468a972f0c5b697dbbbb6d349e42c9f738dcfa4a6b5a5b0164a416f9a1433e38af86fe5591bbe92e651c2f95fe" },
                { "ro", "be4660544e48f907c8d04567adca849aeea40c442296b55c4208f10b1208745e8da88cf916281dde9e27e096581e9b6ae4e843ff5e1d3ea41972b5b31067dc8a" },
                { "ru", "f690385cabe9e8d577c240b1f3513a81f34846d728489d74e05db4a1b15a1fece1afa36962511bcd95564ef36c71c62157f93a2bd5a7f7290f96fe0bc71b41e9" },
                { "sat", "2312707a7caae26ecd6a388d942fa309aad4e0206a68398ad173606ca3cadc070b9d2eb13303a46e6efaeeadd6d9242e24c4504d6d499877a8cb2e9677a147f6" },
                { "sc", "426e68d1479f6127cede33881629f94631f62bfb08d4a238ba227c61ebda82dca277680bbd69deadf32abbf0cc74979dbf26c042246f742f1140158d536806eb" },
                { "sco", "f78b7b2c8276fb15c5133d7e9037eaffaf6b041954ab2ca16a55293cc1ac1beb46fb24010319f24be7e9f04f95afd7dc2d65d3ba56cac452cae2a71b12250294" },
                { "si", "4d2e27013991b78704d6c1e685479fdf7d4494b5ab5e199bdc48c4c761a2d2874cbd0a7f3324b420612967922000a360283c57ebb8e86557c514013446906f0e" },
                { "sk", "263969a98db445d741ed3f8b61900c43c44d0e2c68e56d2f83e39862299906936795058236956fe17686b0cecc9b72d6b8164307e727f0774222b2746d65f642" },
                { "skr", "5de028848dba7a733499db5eb58a25e078e7333d0f34068be0f03cd654d80ad90eef45db982df1ade165c76386a07a13327d357973c7a3cdf5ecd1e3422a60f0" },
                { "sl", "17ac30a0c899ec1ff4b817ace5c1ea2d75891f7da8cb7780e4c41b956954ee9daaba680b8f94dbb8376fcadd76082c46003a5ddc1f8bbe7259ef0a9d54dce617" },
                { "son", "7a4410805a38c8acc1e801f8762b1dc20cc1f7085080dae2c00cd73871f3696ed9e36c228a42160565016cc82a89fe5b0716da4b9545e286b72353609c32c8ed" },
                { "sq", "28570e330c3878190203d54d3f4a748d6ed19054d44c0598bd5899d5c1e6cef910e80f5f8e24b14ddac6a74e45c6d093d815c945efc0df61647aac288d5e45ed" },
                { "sr", "5695c56d27ef0b35c0ae944436052b5d24c633c6bb376b63c98a8d4b46ab87209d001e6166735f22fd19ecbf6f6c2977f4ce1025dca2244b1675b319ab3c609e" },
                { "sv-SE", "933357c7093de95afe5088384fbf032f4892f3eccbc3b1b9ad425c0b489c9ca77bbe6cbdf3fdb2774f91af9957b04d1f029c0bbc3a99c67de49e2cb3f364aab6" },
                { "szl", "d7dec3246802c6d93f71c0404f12e384551fb474fc951d2de28b0ff1464395edd397094087934b6ea63a11dbb9986082f6471d43c38014af604da0e166ecd156" },
                { "ta", "d0c058a265c52e6f5028a5b762152d5cf0e2321ea342f3fac8a09078a066215aad9521c95e933a0484340deef76d81b164880dc7450e4c6bf619e95d411d5a7c" },
                { "te", "a29bc248b7695e1f7d32e2ea0e1cdaa241902beceb61860e4aca3a05935777f2e482b166f321dd22744b42f16a704e05141b7565341674d22655dd7b676dbdb2" },
                { "tg", "dbbe0cfeb5311d08bac6716fe2aa1e9916f45a499a7e9194d996b49c1b032adc221a3df7afad6ecfad16a64c57be14f26ee807b11942a7fc6b5294b0a280a969" },
                { "th", "267e0af0993039a62be1666b9c95874d0b079a53631b3f480e0421054b0ea3224d349ef8b090d92a63b349c77a6119af30bd7f81ecec5b54d63d47bb46d76ded" },
                { "tl", "f41d689ec89626f7c7579171dd51524920a0a8c2625066537f8f2baba267dfae36231805d4a2786ded8e24b8e8dc715234209a05ffd7578b24bdfa94a0d25d3e" },
                { "tr", "a10755af190aecab1b01b8b69be3b456df406f300108e25fa5637bb46650a31276581592b7e66ffc0322e8c02772240f5a8c7df2de5efe57f2db374be5d5aebc" },
                { "trs", "4881336ea56d201d468464c3366d66e1187b18d8bfb59715ab2edc3ee8773ebd22f058320f5f9564e0b3c3273519da62750a47a0ececa91ddaf8a6dedee3e20b" },
                { "uk", "be085259395fd108fa293908bbc20a4dca1f60199881313cffa6fafffaf9676120d2611d588eac9d4c01ce47649cafebb5702fbfb06f1ede0652b555c2e2548e" },
                { "ur", "c500ec8afbb63565fa891c14d02c7d29aa3dc31c1ac0844fffcf910da62f04d35c23a9cc33f072dcacd8a0276729329bc7ca8107d792e7af8a373697f1cd7f6b" },
                { "uz", "090f7dd27d9e33ca05e6de540ded9f19532a1564921b50498351cff0c93c4c079e2591012817251136e5c890793384d54e9ebb72e5253fc55c4882dedada3d18" },
                { "vi", "402da14ae0ab002637a9045bdb41c960f20c6094b606185cab6a3201dfe17f8c990198d4f18bdad584e756ddfafc4fb5f81115d0dd01bd29c2140e77c8914dd5" },
                { "xh", "fd517a7f7ece4d764df38df635f384153dd55b8246061163cfe87e444ae63edf10a761ed1da3de6bdc7900cc8d56f692efd1c8ae27fd3f6bda4dde72f968ee59" },
                { "zh-CN", "1515698afb16d998a8c01fb9fc7d8bdcae28d824cbba95049d01acfaf30b8972f4c9eaff63117f76e7b0c458c446d41a792999126cf6a5066a3a5785dc2032de" },
                { "zh-TW", "f14500b2f9fd4ce94b428312f822dbdbb7efe4b4674ebe13c8d01a1d158bd6f65076d174e4b929386850209c630d8017817a71bfe10447d3a725296cf67c6b8e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "9075323cf1a3fccf2051c5fe95eb72af45a510685d95bafd662806416e4c67759e7d96fe85046bb473962d47e951e8a31762c77cc0197394d7bdd81781eff86a" },
                { "af", "c6242515407b3cd3c986378dde0fd2485d07558b2b2c0164bba07fe5162fc5fba940e0674b9fac4c7ab6b6d38223f4f85ab20b1574763b1462ba71386a8a96ff" },
                { "an", "b57b58571a40a0ca486a0c338610d525fe90742570cdbb6702b92119b1408cca73cc6b721e6fd0839f7e3805927ff45bedb375d3b9b18e7a6935118623e83477" },
                { "ar", "5e4703ae309ba3a4cbf74f8619fa07353b58f67a6c92634d8465ce1921aede6eae1a96bdc279c629e3f4f94532c32cadf05b9dec7b10a04b42b9db2c9136780c" },
                { "ast", "eca64871e174981acc8ce8de1070e98f41662521ca51d930215d7861e9baf6798d2ee46e85f283287b38f9636ad34de7e0e2596e594e238545e8f42119aaad4b" },
                { "az", "0df71f160058a8b550a5ac83365c53cc4650b9e93a7a7f241e8bfc6c0f35cc040b26b6d0027f7ba02932651b3586f2169a7cacf12ad05e20a6c992ee88e5b38d" },
                { "be", "c4c605dd045a28ce0f848e4ca9df02983ad7b8789f1f2799aaa91d06760c9d3e52da9b52a9f224e9f37791d2c48fb89b8e2da95f8b7cf3879c691493c480eccb" },
                { "bg", "e18575e75d24c58a9be34347c389a46a52caef1d3319431a3cf30d13efabe14b62aaad2a77f11e5ef370f3f55b8d8615a5cb2533bb7a5f65001d2dfeda529b25" },
                { "bn", "0d9b33277319e612c6ab339f0b16395e9b67ca609c78cbbfd14c2d347c693fbb31be275a7075e6103a59edcbf902b98f3a8b7f6796ea0a5fb52df249ff682e7d" },
                { "br", "9f331249a44d52eba8eb6b50a1e3a00bfbf57f5b1bed5e0fe82cd197f02d38331e12724d933780467b4ccdbeaf2cd72292787381a8df116643b17bcb556dc650" },
                { "bs", "0c1043eb2d601a75d29cf98dcc852e08c79710912457bf859f09af4413568b6dde3d7d8a331d9ab3a7c8dbd0ac71f1a088763a2a1fbe8e7935b873f45a243a3b" },
                { "ca", "d7597f93f67da83e55e6eb02e73d89d9e71c834afaee27d8799992975451c52a30d0b4c2137e43c39af00ec4cdbf9a3dcae9f01befd01009dff270181b20d695" },
                { "cak", "c064c9a32d118c5328cb32ff9f3b9117978d4e7e724c0cd7f627339ac58831b7b232e12ad089ac1d1edd838436c9d3c61e95a866129b9e10ac6e0d666025da5f" },
                { "cs", "b28fdf3d7ee0ed68f17f0ade4fa1494131362af8fbe8b35674da6ce5073b25cdf7d6c813a53aea80b316648896b68d74c2fa9a22f4c7233ff2495f3aabafaea1" },
                { "cy", "065e1ea66b148f581c2883cce2da15973ec444d8d274642cec158e6c904fc9d604a7d71341733d13b0973cb7e667f5956bd9af5ce5ed3e6768de665e36c337b5" },
                { "da", "a50d94c3c1c1eeac180a529979de945ac4da4aa17d85361ed51065c7ff1bf04954bfb163b92d687c3cbcfe015b15f5a0ed6e508755e8193a9fd57b273a8f0e6d" },
                { "de", "4622c4d22e6d02170ac55d92263baef50883d223ae98caccce9673d86200f92220dc85fb8d7bb12a60737aa268096c5037d68654a7c8c08097002686deb3692a" },
                { "dsb", "df3557215f86481dbf81f121fb60b9ad35c57c4ccb8edf446b05327190c7fe9fe8c09ab4c401517b1a340434256616f0b2e1008bf77112b80a08799222471ecc" },
                { "el", "8957c4094da09d983ecfea80b42e96438c34807ed6e95f6341550a2a960cd11301225e158277fc3b9a6ce2a60a1ba5f48d2406f18983d94318b187b906522e52" },
                { "en-CA", "76c336226b3de5567be114d20e2da76a5b86ae250a4131f3737644bc8e89d62b39ec746a6626d7984cac5229d7786661cff95e469b0efefab6a298d096ad1795" },
                { "en-GB", "893f16e5991d5460f5b646a15ee51d6ea4adc9653cfc2196b90edd76553542ce6cd497eae84e3646bd67dae34e34170033abdb88f011e8ff0f3672a1296ef4fa" },
                { "en-US", "c1fa85f0c54f23d81922fa80233894e60afcb7a936a149c973411c20d6a834b0413cae3273e97e5ff43442702dd56c66cca377497106548448706a8902fdeaf5" },
                { "eo", "9e0a6fa50bc542af674b12c875cffd0e7e59a1e661b465541bbb4cec1fcc3a6edcff3f4a88e4abeedcaf19d4106206c541d5c6466abadc5666017f1cb668bf91" },
                { "es-AR", "ecc6faf87c1a1df3ca96f0480f1f4a34570f221bbcbd5821b2a51c6fddad2c1fd11c537126801de27df53f2f6ac8d16d45af6c98d3ca13fa0a8b5e34b4e5c0a0" },
                { "es-CL", "90872748c4ba740a9f242b0741647a52a93c97b9e6ad821911c2caac5048c1c75eae0a730202a8ede66910cb5fa3e3efbe4719ec01abcfdf060547c1372f0e0f" },
                { "es-ES", "859fef852460aba9fc7f52bca40075f6c17964466488fda52385754500f828345bd9b0c1b8adb3ee05d670f9ebea39399c631695f6d0b23683b122027a679452" },
                { "es-MX", "1e622fac0e54a8582e061b89c9a803c320f4771714edbac77659aa3a5b90cdc7e79816983c63df1620e881ffb795ae28b28ec01b0d9249eddf3d8ecd18f4cbae" },
                { "et", "d058093aedee85014ab03a23795391f8e655c64dd2b1df9edfde1121bf55819024cf2d136559db1776b392554e58adf54c7d40b790c85beb744fdb47eb68f150" },
                { "eu", "6b8a6e468428041e3341659c236447d3ac189a07d5c7d92118034411c4aba0b186e065ddb2a040f9356f004cce7b7fe549de57a1d973bbe66cf8938280c77401" },
                { "fa", "d90f63d9c983795d04bf6a57137e41d520b9a3b39385a4caae68f9e7090faca90f885bf8d636069cf3f2b5b1dc039176e84a6034a7755c5d3485b0a1336393c2" },
                { "ff", "7bd3d47844a87903532661935f68ed61abe8865796831b345758ebde69f3d2cc32d42cbab092bd1c396c32e04e5178c0991601429bce941102c80ee57588f510" },
                { "fi", "59bd95d6e38662eda8cdfa1ae6a9ad1372383a3e417e6677515f8e31d98b16f6d1f6c656f5c7e98c7701cec4f419ba004218e47fd9e0f06a75789169ee1da07e" },
                { "fr", "968e44d9419cb0833df0068a19633f36cf4180dd140304eb5637dca8b533d936de95c91de91303245c171f3e44ade8e01154eb2f5fbccb101712196c27106545" },
                { "fur", "4fcd25bbee959b37a5af96607c3960925599b88ae3763ad002277f18df325af53c6ffae3d4acccccd3d1df08db0e4752a46fbffc1a828426b1a0c6a91eb32c59" },
                { "fy-NL", "9bb652f55d5cf2f88f3def58f409832b3bb9aad512d2226bb1bb6ba2fe2b177d8df42441551fef20372623b4f82b7bb3b9a1d8fc5b29af2502a5e8a02073a250" },
                { "ga-IE", "c40ecccd7de70afbd1c3c6679297c8f13ab33d3dd7af248b4dc2f1bcd6115793ebbdc29839ecf7edefe0b4555fae90109ba0bc3693047dc1ebc66b71495952ec" },
                { "gd", "3f508b9beb25d490293a9a1e736d87b63d992b66301a98a2f8fbffd960f546ca7da7084c75abb3752d22df2018ae7c9a305eed014a2108a04ad9076c48dc1ac7" },
                { "gl", "e3f6713998f96395211a3b4654306e758b9bf79018aec489572c92675f6aa0113de5bde2cd0188f360599c47bf3286ed886cfa962fdb83334d298cad5412e5a4" },
                { "gn", "4c49df2983a0672fa87b57b27c45e0f5a28fddef0f5d033ba652409bdebcf9cf9e5abb102c0250a22cc75f7c5b85ba0515f13d7d4148616a6a085a310f45f513" },
                { "gu-IN", "bbcf716559e5624b11785670f1360144b05658c3072e9675e6341d967ad8398c3a7812267232f8d5aa6861fefc53741d0cdff2efbfdb17ec6c20e1a7c1c8e0ae" },
                { "he", "c5ec893e117e243d8c812bb1f2efa4b3873e4012013255cd0026285a41029963f64c70c0822d212737f5f87610f70a14026e2ec8166052d52d355c0c60c52c6e" },
                { "hi-IN", "ebb1e021e6970f82ed2411e8530e812e15b6839f7ff715dc2c9a2ef2c4823fb3ea27b92e0a6931fb1f1a944c309860e16cf36f844fcf32d3c5a75b31573ee368" },
                { "hr", "0a839602736a1d6ef781242295674336351ba4b1a922688213ebc6d9f8ccd3491005b3aa84be88e52054ea7bb9507c91d2b7b630a3399a9e003b6dd96e848ebe" },
                { "hsb", "3f81cade4d2baf3e542e067cfb367531adb9ce850190f74f337e490ab58226dae733d7f355cebf419031f3cf9d33283603067bce6ecb7b32285d2ae20e8eb7d2" },
                { "hu", "0170fe38457c64736d400901be65ad18b35cb52efbabc2304c0b936001bc4079b5fb34b278eb70ee1e13934e907ebc5cddc5787abbe4c709d99b6b95e16a98b7" },
                { "hy-AM", "a1750f94469bdb229b366784e9b4941aa5940ce6f75fcbbdb36acca0ebbd1db15765b49774b45ca9d66ad0a3904df619fd72c3349c575a63eb080ece15933aaf" },
                { "ia", "80d9951ebaa13bb704f64e193287dc77911f7a7f14ec9756b5c2a33c8cbcc65507df23cc8b36605de585f68c3c575ae1cc09925ddcb0539f853188698023f225" },
                { "id", "d4ed06d99d6cf19749e61dad5d3a49ed3092abfbc9c2fe0161b22c3a224ff533f7cb05f0bd1a36393c0e5603c230b0b77c219b00783e3660e82c79758da59985" },
                { "is", "546126ad3531c8866d1a080ebbad3eff13a2d6adcfe91a4b92151889eda53d2f8033ec0c2fe26d8717e1f12e183c3ff03ac072adccda7e96a7ffdb830f9d4d67" },
                { "it", "a16830b6547bba2830ba3ba9e0d6684160f85f2f0a4b9e00e74d6ab0330443309c72875e8936d486a473ba398dc79182ecef00de3ce4a3a3d4b04c8eb001b904" },
                { "ja", "06de7fba0ef6c3abd3f6a018b3f36fafcc50f5491367cb524fa42011acfb1be66023f77b64e97ec32b7b4e26be3d9b319de0ef70479a6239b8bc206ec730c9ce" },
                { "ka", "5a5ec7c515c0976cc9d8ea82badc5233f66243f87c8212b9d0022231b76a7e5f362dfc71b92f41f88456004fd62b84d9e376e7f6ba2cb2e321cf25ba7cbc5e8b" },
                { "kab", "a82ba72a5a01c10359a03da4c03bd6e1090db14d390cc5d935025c11f9bc1b1d78cdc2debc6645e344dd53771ece8f8e8fbd82d606d2c6446b0f2eacd69c5562" },
                { "kk", "a2f9865913cfbb0956fceab3cccee82f1148550dbaf36c285f39fe70dcbd728253c742f3a3588997b6cadaed31840921709692939265e781916eeccc7a13e308" },
                { "km", "8d4d1af53237965f996a7ffbc1ad5f4514e2c59d6295e209cb6934f6cf3cea34ed2149cc683dd3509da39c19773cc4f36ed3963d9f748b1a2c0c80a2107adc75" },
                { "kn", "745d3c1a79b77da3c71a036216757a9fd8ffd8481029fc351b0da4d21122e51ae847d18b9371b4270f899a525389891439de5367f875fc56578a2a335cd73337" },
                { "ko", "13f1d0c580a34c109835f9a38794a84e307049374fa61d65d2cd3f40b376eaa15040b37d992ab964aea2b9ee8d7c10ba8d5c428b67066b21cae187837d5a81dc" },
                { "lij", "7c6ca78066a2171dff18577aaca95897e82e5bc29bfa52fbcb59bcb5906a6d7ca26c63deadb3b8b7923a6339d254007212639d1f88c25ca5f6f0d1d3667ced68" },
                { "lt", "c4dba115758ad9505e45ec5badb86c458cbf5ee132ec4792a34937ffce04e9d6c2e836058e0d955d61e61909c9971811ec8a3819fe8b41e321af5f56210a2d7d" },
                { "lv", "985bd89f34fba5ad3e2bb8e22c586b34a407ab5f7f59abdfb6090fba65fbeb96e5a6cc686f43cc626843b53093da0ede59064a2a5384362aefd086b38280e7af" },
                { "mk", "2c9aeaceb18094cfd58b39f4052b1c082fa7fbde6a804d87856285a6e36cfc3c0eb50a115176133d5e584223420c3e98a9537e941d1d48af59180d538fa7bcfc" },
                { "mr", "b3becdd21df433d54b49cda78cd574ec1b48fb19b0650988e56b8b6e9bccaff85dea10df4b12a1532c3ab78d974714d48096b71b9d0a7847e0b4935fc551d7f9" },
                { "ms", "de5ba5715ff97acdc2768eab58e36fa86929acf03e3df06ba4400a5312354af805bfce18c94c80d0f4eb4a25a22354477da3ee77d1be1fd8988873ed96be6e56" },
                { "my", "e872495c730ce13f62029ab4ca4409d30250b764959b14fc4190d8940e2a02252aff5bdd9a51d8aef6b895b1e8fd567320ded3f7065d01733ed8967646459419" },
                { "nb-NO", "f4ce4b5f063405ec1ae5bd4be97beb8a6f7eb3acf9b9dc63b06c0a4ea0e974b7bd321fc9ef3f9bfc316584bf03a3e05b7e8cb5e44cd0e849485d5d52a1e5d2eb" },
                { "ne-NP", "41db8d79c14499d5f04955a065d0cf4a20fef82292eca2e832182b16716d8c4d831ee596c77b64852ac029702f2e868b44eea24ad1678e1e7fabacdb7a472167" },
                { "nl", "52050b6fe26dd29976f72fc3b411d2cb4b156ece01826d2c4912b717ad8dcac5c2b140e78d93cb6145a117c3c0cad366ff3bd0e51860ebff5ae57db8a99b9ef8" },
                { "nn-NO", "ab6852a46816200f3dc2344fd1d9bdd3dce930d9df611b4f90847d8c4a58a21fb07c1033bb2f856f79a93e2052466d703a71df27558bc54525b219eb1d7ced5e" },
                { "oc", "e8674202af3109f2b787710bd68b0801465883819a778e102d94e2b83130908da0daf17170841d53c6f98d79db358368f61f9cdaba5ec5ee1a73eef1ab380239" },
                { "pa-IN", "394234f16527988b3262e560fee014a181572ef78d639df18dd5b6d52db65d4f206d777e0b0fd03c8a57d91742aa4e2129352a0a58e9a07203fe471ea76e6f8d" },
                { "pl", "065af5af5ebcad6506df125c94fb45847842d83ada464633cf4df972a5e40e2f5bbbd956a09ece3262c17adc938fc1c25d8370ed0e73ed20b81f408739cd7689" },
                { "pt-BR", "70dd077ea50fdd197da14c27d1b012c6c5ebf726b4f9cca62c84cdbd81fab2bcd3130521169d8465230de26b2970175a6ee30220466bda526d4a83f3b09e8c2c" },
                { "pt-PT", "db92e9bf7a1a32cee7dd571e1d51cfe956b1f52bd66a4c92c13c1f7fd9cd1b0dc68c852d9223c9de7609cd5b6939be3569c8b103daff9e9a7af7302c990c20bc" },
                { "rm", "8b7747ecf3fc30424ab7be147e5b519842924c16eac3e1a983af2743385e2bf12e72496beffaa07d3db90fb776cae6ab163ce884f2389325afc22fefe1249c7c" },
                { "ro", "d0c914a19d7e82d2b14f5384c5e7c22c6551e844c59800ce11ba3a5fe2c60011f233134f458f4c11d1e4bbbbc49909e4c00021170f935703d111086c16e51fef" },
                { "ru", "ea9d24ff19191638ad6f972622ec46b94cf88ff995a8b5fbbd2190f90d5b5f559a6535978d95b286cfa16de801112cc68b66ba167f35da3e95e81886a0b09fd6" },
                { "sat", "9fa8ddcd2e12287a5b49bb55f6359498cc3ca79ba33c9b91e11ac5edc6a168c648738198973994bcf089584a76ca851082e1b2471e0327ccd235deda4dae9259" },
                { "sc", "b973abc1802adef2d21a3fb19b122ef3b83a662b1d8c305144ed4834c85eb5c42b57fdc71473ba3bc3e5651190875abe699a6cfb23a647d0bfe926378bb5f4b4" },
                { "sco", "e7c93e1b4fba5e008322e19cf471e3764a94fdc78b528d5f8e92efd5c11ce9d4aaf7c3a162644339d05b3635ce91cdcb3dd8784dd4c48bf73055a2dad2d3967f" },
                { "si", "eddf0c3994f88cc859c32daee5e94a4ce0d218192a2b3012d1ac5576ef768ec205e00797497e30f9f46064e65e141a6fd42a620a7f54aaa62f71fadbaf0dec19" },
                { "sk", "e9c78e1c3af8f3010204af4b958e630ffb894e19bd6abd5f546107458b58665e242bf43272b7ee7fd82e8e14417e193ad938d42e0f1538d6a3720676c56abbcd" },
                { "skr", "734d4e9cf6271da6fdb29eed288111b921f41e82700521939b5c3b24d48d3f898d5d41ed0adb28f1c2278883a7edcc10609843d2492151fbdbb4b274f71f3e91" },
                { "sl", "0ac88f5f2ef1a526ab78469705dd0d4fb1c6c9403cd29f1ef5c88c748495f25d07a12e0a26bed229dbd0be2b43fdbb7ef5aeda9d89871325b9b7cf24314eb385" },
                { "son", "6029124e7464e1d0b677baf98ad96cc7a286e7ec7ba2a221ec723b41a55433d8116d27da46188e10ab2b5af1abe4bed36c04fbd088e12c136e09a4c5ac1284b1" },
                { "sq", "d3f0ad8c8f4f4db94314a75648c5c857dba099125c59718e5d6d10c83cafe78577bbe272464b5e2afc166bd64be04df3e446452310994de6cfe3b5fbac3d59e8" },
                { "sr", "f767677d59fbd5fc770e9bbf48d7504cb5313961ab9f97c5745c26ef987758809541b926bc73bd9580c13d7e89450c75ef8d110684bc9cad117f04d386168c4a" },
                { "sv-SE", "230bdfb2aa3507d5fa0376b3845ba373596d84b8ad46e7fd6a054930676cb98f81349f4ef7a100d47cd1d671ae59f6872b21d0274d622b0aeccac1f8325e44cc" },
                { "szl", "4d9b960c11c6ebda91062776189087c52edd1ac0d6d79075bbbf2ea4085578351146a9c7c54bc5f372a8cd691600a95eec8a846d7681522ec12aa74635d1e3c5" },
                { "ta", "b5c5872528aecf6461c10e3025adf5c1c7b8da6b4c824bd52186560984e3cbda944ad30d5c973a73dd83c88ede1b37aba35b88fb43a4d69c58f35eddb4371090" },
                { "te", "e2cb197aff9c3efd2c3cf763a0ae101f24c2625c4f99f08c965a68c35dea0b2a66587c8e9442ee690a41a624a0265587a631b51713a535b57f41b3091c6b945d" },
                { "tg", "d092dbb93d129cec310fd37d4a2f3918eef229a32a474aca7ae6468c7c9e82085889fa445f2bb6b59616aad420ecb35e205ecf0383467eb72c2b95346cc0a06b" },
                { "th", "c0046c29702d559a33fd66ae391b047285394ec3a28d6b0499d9b39d644120f90e784681838e0c93a78f405fe346c6163d3cf2471bc92c0cfa48448fad3cac48" },
                { "tl", "205c71d77c46176d3cfedc72c755e9240ecaee1e988cf29e1ff1e1288e6bcde970a2c4cc6d96a2e613c7df16012d91deea9bd2189705e4206e09fcb7e2c8f032" },
                { "tr", "6feb96799c277b2f8b0b2c07d78df70168e9cd7f53a38fe1c0d7c78f2a2ba3031b9d298f42308e73cfe3adb12b10663ae848496ee6a552cc1d7d8c09dd9b9b8d" },
                { "trs", "ee5f9e54ade04a8dab4994a0c4d551d6fced780f473def9a68e8fdba07d33ad95e4a8bcc600f9c2a87a35bb66462d6bcbbf6379425b4e466575fb997ee0c8bf9" },
                { "uk", "74fb8b501fa92e5a96d800bc907f147088f07805e51fb09c5133f6732bfc040a8b12dd9e1ae466790163d2b81e0761a707f8c248baddc6dd70cdd0dbc9feb17d" },
                { "ur", "7d36f7bdf0cae044c1e9abe27ebd03adbe9b13a1a12ce0d58dfe14b3717791c1ee07e8ade9d0b8c234732074b5a582965ec427232c3c81a198ae9ca4c45c118b" },
                { "uz", "ab1f436f5732b8c09279fbc9e248c1e31c998573ba0031650256395cc54a1e58f42f2efc187916fbc612ecca477139fe769a8b5cf266a48636c75011edc82b00" },
                { "vi", "008472d9902767711b9fd7aca97de0bb51bb64f6182c3bebedbe93135d65d3bc739d9927dc39639389e2c6965202468a17a85bbbf0808ea75a6d20dc9c7d786b" },
                { "xh", "fb0925634746a60a3fb0b8ddfe3ba6687ee23a7b2e4414a8898ec083c74b7b557fc4ae023fc1baf6a62a8370f4a0500209b6cc2e2d594036ebac823d565c9a2b" },
                { "zh-CN", "3621b377207bb4354d5c1bb6a44596f79d1c8d09326f39d02768cf9b4e8af23d79cdb8eb07f305ffdeef7882382745114ebb1e8ce565711783e40710cd7bc119" },
                { "zh-TW", "7798cc31b8e7e015e4cf4f11160a4af4fa5287dfbec5e292afa635aadabb6279034c18505e0943fa168e2d22b761377bc748628a9c40e056236b09dd45579d3e" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return [.. sums];
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
