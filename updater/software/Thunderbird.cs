﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.13.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "70456e572f2153bfbf0062c1c9585232047f4dc2820680d3b21d9cd6906f149e9bd9134a314a92eb5ba3e4e9c39fee6f9dd6372473fd23778bec370f76a7a273" },
                { "ar", "2a7770c5bc010557c5ad1cf530243bdc24591158432f1e10e336e2b146274be941cfcbe385a16b8b53d52a0943ca51f76cf0289bfe4a49f61196ca4e080f8392" },
                { "ast", "799a78fa171decb4657d24d085f329042668ca0a2ed00cfc6e2b793a221f49a942c5810ac78b84d8f399bb993143b6282cf9211fd463ee0d353443a2ea20b3bd" },
                { "be", "563474042903cfde176b1a089046d3e634dcd4fd56880558acc657cd773b5480052f0a10889c02d38779aec3cc9d4f6a89e069c633b073cdd4aaf168ad3f0ac6" },
                { "bg", "ef28afb509d05119c698ac9627c98989ec1d589c8c8b5c4f147fc2619fbe579cb718bf499422bfaf8230e694b0a51a855e8e0fc7cdb4e17cc414d1171efe549d" },
                { "br", "7a0e2f778f987edf4ca0a41eb5713e31bdcc655bf2f674d35ffbbc770d08fc00d9e700280c430f94e9eeaaa7790bb409f207fcb2c381d6d262f75a902ec5b24b" },
                { "ca", "b4202c187b100ab5551c9aa7e18ef6b8cbf0830fd0cfebdabd3118f90dea112d20f18fa0f6e6891ea1252174e122d72b94a3d0c0c59b73b3a6d12fc49e87d290" },
                { "cak", "26597b2b150b75c9d88a97fc602ae1237dbb60e4c7130a87b6d9132d9fa527dbdb02471adbb497a2e0c85e4b6f42c1a8f8ed0ac275f3e07281cfe4930f52c7dc" },
                { "cs", "a07190d2be92d8b91c170de292db863c6a2e597f484cc980956bd1616a7b3587ad24b159d7a4041ea5552da9a1f18187a662f2e9840d08bbbe82285a21519653" },
                { "cy", "3ebf028ad7cfe449f73e01c4dea2140f65a40931f9cc6213bd6f2d4833516cf9d530459902c71be1cf97e7545028fa063cb11688aee1384e42eb1f3fc4d9e188" },
                { "da", "1caed8bf30d216444373f04abd73191f8c1cf3292f9d7abc073360e22aadf6417ae5265e35ac5d988eaf057d2deeba477af812d1c175ccd9b9ef02ed0ccd616b" },
                { "de", "e4c45330fdc5dc6a902be2177c7b6e9e9b5c533a49737fb8caad11c46ee37eeac408fa0f69b7f64f0f7f8b3f34bb49d1e885d16b06679be4d3dfc39272d791fc" },
                { "dsb", "86071171478d681a75f35c03baadca89d6fdff890461d26cf125ddd86a44dd5abe1f2df3f654c7e67f99b7d6ed76a12c39633e674f992d452e8c41acb52dca12" },
                { "el", "9ace8a671e036cfcc238c21700ae513f42bf9c825286620eeba1e3988ec5c317de04a5b9fbeb3c96a1eae4cf09ff6156c0e39e82bb472858868bdf2b54aac3cf" },
                { "en-CA", "a7b654a48756af547870c35c598c7a213a5ad77d563ee32a6f9f5852e3491ec0ccf6c5743014c78a966214f8fb36556b32de8dbe8fed933b807a5e5acb445586" },
                { "en-GB", "09aca8febde24ad71076163ef1d0ebb8494957a9f80c140804a065b02a54125df857f1af93a3bada065c08cb40b8cba72215bccb814271359967a61f8252acc6" },
                { "en-US", "aacb87e20ceecdf169c2c142b8fa553d0325f503657b2e5d5ed4978b42bc4d162ac1531f9d2921984da3d576438ab27c6759bed791687e5974556520b8db80ac" },
                { "es-AR", "2b1841852ce9f7674978662c4267d886e2dc345b31b5f1bb591d6f873bdee7aee537a7f40e2d60083f6df1e86ad385e8ba26d9975c7b8ed6e4d2fdb9341e2b6e" },
                { "es-ES", "7c27a5b41b27930c48d8803e1712f79cc8d972aa03d5073e3f4f6cd680142eab261c5a875ca691fdd3027e36a9c29f538f97c678dbe1b75157e12c02d3eb4b39" },
                { "et", "4f4b41003de1aa251d9088d4dbafbf58418843c9436d28e9598eb4ff35607cbc6ba6e22bd119eacfb313da8a9165f781d5555afdfc5726989d7b7174724c5520" },
                { "eu", "8fbbd1b7946c26127b61d46d5a76048adeea9428f999a4b9c32ce4a74f81841ef809beadd65e65372056d30fe8e6e2a9f6255f6a07a2e41281d798da753bc48d" },
                { "fa", "0b6e2067b006c999299b88fc7af7f705758fd6278cab25cc489c9f8db091f94f7e73b77164602fa9d59f92bf388334d2a98ee30e748fd620ea21d03a588712fd" },
                { "fi", "dc54643fc9a579818c8b45d6627580da50e19f1be32f16550e522d0354632e3af1b921a9994b33ae69c0bf22806571b4f3f10307a1b0b5a6d577d4f7002f4e05" },
                { "fr", "28d20fa71169965e2c3a2639a716394a14a51f73e35352eadf21443a7a796696e3b338569cc2d49386869884530e998a993e908be374227e0f3fbba2f65c0d41" },
                { "fy-NL", "45dacce4f83e26107cf0ce62f1e6f802d6784644db69c35c1dec259126460bf7eecff2e8c78bb63974cfac3cd24eaec70da7d818c8b9aef712e473a98f8c78fe" },
                { "ga-IE", "29a0ce2fe1e68c45da37eceec1863c136b4104da9af0bcdcd1ce5f3dfc3f622726938a95553e4d1c58ae1680e87584fb6a6f78b4625749eaa0eb9aeb1ad7fe78" },
                { "gd", "3ae155a99f573c74794139549945d362d90d9a6cc5a8ca6b3241e08af51a2c5ee1c64324088ecb6e9fdb80318c3c8ba32a1d18309880e191c7eb62647ed9b55b" },
                { "gl", "5a7b00ab39c5c0e7282f0922e5b30f7c26c0cf04f2393e5b6a32d4dded163015bdf32a8f488ecca712a733cce21545e8d23a482304343c44ae2c5ac8044608b5" },
                { "he", "78f44ef9ff95c0e1fafd4d208e4144587149ec11d1a5f525d6ed486ab847e643ef2c08a5271cfea647284b08ddc290431d216fe233616be5479da1bf55e29889" },
                { "hr", "d35f75a496445941c2c661092efcef201301200c8045cb15d3febe36155ef3b17636ea4bbdc51b1ea5b9bdf219e88eb4e154c90281cda6779962ececd1df7e67" },
                { "hsb", "98f7a0d006dce27dcc47052e70cf4693a7235a53b6a7e3966a8dcfc96b6b48f9ba62068e39301b0d8988ce29fb15ab20287d8df9849fe939abe8e8f8ddcb89e6" },
                { "hu", "ef36e3a91c4f96a9a5e6a1ff5b4d701683b56ac8219248a7b0f3c1a1b6a185352e0758fb6efb4b194b29d15a0bdade356d72d9abcdf63893d25e9a27e635748f" },
                { "hy-AM", "a05e31d516cc16ad1bde800ab5652ce5eec453be7f7c67f2a98021c87f12362cbd2384c631f5b16241c5b5de162dc2b04efb4a1d2c556ec3e04e8cf878f3ce05" },
                { "id", "189b8e90b71a14b775b32beade81d3b69e24c88566ef999a1e91fe630ae7fd4bf62c047304744d984f9a1ace19374f7ab36f3bb2db06b22044f06ab1775757e7" },
                { "is", "1f4271a232fa32499087e181696bcc06e481a616b00e2a1eccfab03d27eb3dd1b50df8661e8e845cbdc5b1122a0c39a8cabb66abd2fce33538d1e8f4bbd5805b" },
                { "it", "99682f6286e05e7c8d339804d3aa03bdaadb556402eb336c22912a5d050f7b39719086c34355fca1d487d0cf21556a1b9d589b1d0bcc75815d5a95a860794ba5" },
                { "ja", "66e313168c9d22e3f0ff396cf036dfc9acf94ef40411c0b25640b8687befd7c601f7f2760b5839c47ef14ca011a9bf30ca7752be2135b357051c929082f04a74" },
                { "ka", "dbbc8571168e2c6e1141fb12de818365e7a3ee6f053c247a561a0e48f0b9e54974d63d51f2f1108096d16b47f8b9f24821afb58038db6b35b82eba7bd984c141" },
                { "kab", "4df98546121ce4c8f93d10684c4513b2a4179e77965ba98b0645ba1a7c92fb4bf6602b84d6913ab01784e323c5484f1f09655d5e199041cb1511d123708ae964" },
                { "kk", "46bf41c11d5bfa617cd85687b93bd0a6ee6d7b8a2130aafdd06dbde6b4a2b808ea0c0a4bb6aa7f856f3974abaf3eccae3c6faa301dd78cb4219e6a5f176d4bed" },
                { "ko", "4dfb9fa10ff23dd4e9a3432defe734b921a08c9fe5eea1d17f84021cf093d4f410a1a39787f7912495990ceedec20f4d7a9465cf9c5b73015ddfc2b4facfb6a4" },
                { "lt", "356fef72d60295179ea87f27200dfde594883bf4e5ed534a3a458c9e26930e33029c8a6f5916538620f41830ac87bc55ee13a8bd130ce72c1aab80ae83760e2f" },
                { "ms", "9b85d1cba76f2a2d3a4a6ff81a7f024d7b2648de4f3bdbf35194a587443ac3aca1e0a56c18b6e99502dd757be7da00dabc6cd33f79433b1a357890f9a251e3eb" },
                { "nb-NO", "49e904d0091bc0d1ba8e32fe5c540dd575b83f4da787d5bc3efa9e10f6cde471da5c68cf1498000909c13b345c8a13b45e9cc4ac94f2dfc74738e1fe26e10763" },
                { "nl", "eccd22e510d7f9d16582462631a88806993ba04860187070a5752b5640f731b497d7007e18685ad7115d772cb7a9a40afe7a76127d3a0460ec8068a28bf28f9b" },
                { "nn-NO", "59bd14cef5aa0903164ff9acf2ad3da16d9f42f4eaf67b657ba99db472769f5ae2c43609c5e53b767d4cc6524045e01f971aa950648d04283bedf6c8785f74a5" },
                { "pa-IN", "23da18ce0b86dff2b7b0b37244d4bda31674cc170b9c781e80a0d1b122037a856cd9539c4c1e204e550f7a752902d04bc1b8bf92455547ce9874d4bb3ef65199" },
                { "pl", "476632418d59909d051c8fe6d73f64e53628a2c920a0d825cb1e878e5a3354b090e92ee29f8f8f6824db10c2d7ab17c7a05e0dd619a9674c76894eab6124b271" },
                { "pt-BR", "3e3639d61b1951a9a7528b10a9a668b85328e86bc69071234a42b73dfd9c65854e05a7e43ce3392b3589b3ec005833cb41f3aa15880d3f126983a842a5ef9eb8" },
                { "pt-PT", "e6d6214a8da7ea05605a13c9c57ec2b9bb587068574ccfc8069420dc13a9d524b2e23ff780c8202a427c944f17a9bfc5fd5ce990c2d4d67fa9dfe07e64ecbbd4" },
                { "rm", "2589b4a4fcc77b4337328073babb2e1e035dd327f90e2cefb0bf65a2e127d9660bafafe215ad602d34063ca3f25c713095f70e9c4dc97ae6b6f75d5c95bf7772" },
                { "ro", "d75eaf62f577f0f986ccb949c034720c31905808c7ce4b35b876a354d7b1cf393b237b62aa8a3a91b4249d6a6df9280bbc71680796314fcea4e2d14bc7bcf6c8" },
                { "ru", "855628df4137dfb4e87b86c827c01437654759869ef65dd5bd180992aa7d363328d77684588b8d556ba3a77e81a228d0e49a99f040ff58a2f7e8c0716d939b93" },
                { "si", "3ef8594bd791cffe26ca08273ebcd3cb4d6a590d22284d1cb27a4696c9c48a26260e27bd582e3c7d5a40a4539fc326dd9edefbfcbf402212c22e108bae41f955" },
                { "sk", "14e3a081d3e2aed716f43c4ab2e3ddcda1c864681b687b149edb6cf34d327a8de633bbec08593b6be699522c5a844e293db6374d53a64d5102da09f85f65f69f" },
                { "sl", "379e3726a92f41aec7b97a19e90a58e699fd71d85fef9d8f7864a90921c9ae67ddbbb2809e9cbebcddd2e5e01b3a8286ea5d3345961d23cdd083a98397dcc4c3" },
                { "sq", "7d0136ee88dfb4646e30fa956cb144213f59e210bff1e61e03902e0ad88dc2765a7aa813d0b7be7773f8bc412d5a59eb28379ef15e77ea10ed85b9ec26debdd1" },
                { "sr", "dd834a48fe0d1bf8649444bfb5f9d818ced44073eadf84d827c0ef27e8d99fb261493f77868727fa8aba3fad685ff609d248cec94de383f7dfd1fc061b35663b" },
                { "sv-SE", "e53ddda939a0a78ee5a60226a4243b116d9b0b5d4cac903442e92294542d4c754713cdb6d0666753644add7fe0bb96c34e37521bba16a251e53fcb5a29f48653" },
                { "th", "08f4b432649537f90e6336524e8364104b565f80e19e272a3601e82df3326633708ad7620d22d5171b028340a6765ea7efb7f9972a595ef943917e039620e1b4" },
                { "tr", "4ea982b9c34c1b8210ed60f5bb764df7d47d8d9856526b1c72c78562c7752ec3c127b5c54f80135b2abee55250de697e18e0a5c1177927ad742ae87fb507e8b3" },
                { "uk", "b3e5ba7a1b89cf2cf9065e6b8126a85ce5c10297f7bde3e12c4ebe3d5d172afda7fe992ddfd5080ac100895d0f19dfe7c6c30643f456e793a05434e9d7d79115" },
                { "uz", "cb57276344a952a7b4ec1677f59900adbd38e0bf924ce4bf43089af21f04d546af5c546a6ecdb5bd0e88ceebed85b5f029a8991617f9f2458947796a88153548" },
                { "vi", "23b7a72f7572eb1d4f084b3215a3c6e9bab13d77078f8ab6daf5004ca88fa5502415d20efdbed91baf7fc80b0cd871b48036c66eb5796a01720b34728a69fae7" },
                { "zh-CN", "5f21c41978ab6b8ad67b807ff36e49fa3499af37429cdfeb8abde8ad2902275f15efce32c19fa284f64ce8237489069197abdd9366e8d331f2e257d2e14b7392" },
                { "zh-TW", "6a51677f89db9431d002706cd1f7ddade4870a8c480dea13cd3e1c5c2985a8f961d79c8155d88ff4633254fd715c11e8aeea6a5edcffdcdc79151a3a1dd8fcc5" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.13.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "46acc94a41bdf87705260bfb4d3efe88a49b7f5beb5657b034b1022af6672f1d4e07d133263027e693950682db1631e1faa990e5507889f94c3fce93eafe387d" },
                { "ar", "52b9c5e04bd4ad3177453b066e6cf9e965f046956ac4e79190d918de5583928de0e66fe7e83ef8f836828796ebe92caf144f24400d88d82127f6f70de08a085c" },
                { "ast", "1ae5364f91e3b666a46b6f592a934ae9fcb7963beb7f3f6458a44fee34de8da880463d2438e1d2493f358a484b2619d22c1040384e3ad11a1719a9b254e75529" },
                { "be", "859d608e2105396abbfa3bcf2f60cec7bd51de293ffb57f8aeed46f7f352f89d4811d240928fc425059efbc09be7d5ee3b78e16cc3d57a3326b4f995b50be555" },
                { "bg", "005201f166d6287c5cc3fcd1e76f833c4d07ea979f3b28687dbb1efe9ff29247fab72c7359d4aa25a3bc1c3802ace42734189b65834b63cea007196b7f924738" },
                { "br", "ea41f45d28e83691f6f63fcda4287b20cfc175d621c320e1e75c79925022938ccadc1bbee9502b99c91034e072b00d91da2aa5930c110e4e43bc797f97f8b148" },
                { "ca", "fc06164a191afd84c2f6b5266b583da66685e30947e6ce04ef3292e5366f53695f4c66d961a3a1a955ca803f0e286a35cb75db4722cd1fcd0e1f0c81e3cb9a85" },
                { "cak", "f10e76f3b5b8d44c2e8e6979aa3f809b523b9264c4a7db545d988dbf19d10b48ec9962290256ab44593d60dddc97b98e6e320da71e7d32fb8135b7bc814f8071" },
                { "cs", "90fbbaaafa53987eeab30222a7df0e6a475ca0daba279cc43e4c7f9f1554fa92a9813c13b1ab1290ff87c53f62831dc40e6e5d4071d2cf6aaeaac0d8d86e5bbe" },
                { "cy", "4b85b097ec80085f7063a2a0467050fdedc99ba897b08491402051e754ee1f21ad6333cbd40ad8c00f9fd81edbc895a39fd1d7ea2dc3d7db6a1bebac18788191" },
                { "da", "65e3999e4d2b0a4038cdc2daa2159380a86e2e76c545faa61676e123c402a98d23cfbd046a88913a4b27d7f08d1e7751f5e7e3fb1288741d30a50a32153b3499" },
                { "de", "6340cd0bd2febee1cc7434f16a51709a8723adda02160179d1b195892d2d209cedfe414e4b38c5c6fc9f8f7594107778b734002fef2fd719c390ebf237e10249" },
                { "dsb", "97d8f86b8dc33d6197413d6448a5e31de0d368238288986b35c4ba91da375603c19d786f621704bba2751d013849b7ff95fdbfb9c65c4f5a46b34bb3c55f8402" },
                { "el", "39d59d3b5fda99f24c109a5c0c6627648a452bc62600a9bc16746d736ca42d0198a56e8089e4d05411d1393487268efa7f1e7657b267ae9efd8d54a78a631bee" },
                { "en-CA", "4b6e00da719a7a8ff5a3c4a38b74dd60f8a92af04c597ac178efda3f9da0a4cb7b56b69aaba5d3b8db8557e7645cdae59e02e350f79a509f889e0939207679e3" },
                { "en-GB", "5a4542b89e969ffa7d2a54311d3e99e3d4580be28bdfe5217cdeb86b062dc883f74118ab4c2f48e76e741a6d80c010d98bcb9db94db08e0df598942f549b6214" },
                { "en-US", "1bd0ec67e3ca36bfcf10fe0499f83e71817066ccdfef0c9bb9d23e38cb3621b51a1b303ec01444152075fd30c2263b7bd5535469257d0df0dfa10f9ea384d51c" },
                { "es-AR", "9a53182ad4139694b0fdb14610fbaa474f8d2b1ffeec1b2728327ce6623b5e21f5713bc19480bf6dc6c85bfb7e0a6dd14e81c9cbb1fd05da6df8ff68fb4f7da4" },
                { "es-ES", "299087ae47470eebc0034adb0d8222c5b9911501a51803e5bb644b7c5cacb3fea696aa4ddb06fa81b126c81972d5ad2e51aad781375fec2d43c6f2c8f6233b82" },
                { "et", "f0f1b365fb0f0c624c692c1756f2e843ef7163f6e7bc833cf678f4c0f07106e8ac21dd9107d0b038b67d267f99a3a6e8e675134d91411de05c7b7568cb7dbeae" },
                { "eu", "e90220cc80af6386ca50435ff57baeb24e84a2f97f43c706c4143209b6b7f7dc468f7f7d17f4f8e46556d60646c85a9c4a25bd92951ce34dc16ac8d5623f62c3" },
                { "fa", "2e55878bc5d226f7af38bb66673a8e88153cf54920ea59429f79a2910c9062a123accc191cbf7a3515ff7c23387dc7102bc999eec10b0386e6bdf96b2faa3f87" },
                { "fi", "f47b91416383640db4e900af8b0a2cb6693b8358735b67b5050489612cba4342226030de57142931af64877ef2da5fd37c10d173ef4f113ad02496404a37f908" },
                { "fr", "ff7a41ad258223f7ecdf778b42fdfa0e50984c4f16750a8a8a162253eff701925c8f0b8b40e56d2717f7e64be4643d115af9e9e5cd8f90bbc9991925be9d2257" },
                { "fy-NL", "ed4f4e2e4b56302ef8289b2e977cd21816d2a0395a6396956c54cffbe819fb4492cb8a7ef2a938708c406c756886878e62d146bc68d31396734512c86297c98e" },
                { "ga-IE", "9415f7734436c57f4a3e416335eab57e26b0aa275eb8e4a8c2d890b9c0a35eabd780f63e8d28795cb38238ac3948cd86d6f3a44941330aaa9bfceb0d13b18f95" },
                { "gd", "ef27d072f2239267d020ca3d3c3e8d4a74a2e8f4b3a7d542be2d617aba82eef06ee87adaff6a1788ff8d5dc9fa0570a07f33f84b5882411bfaac5957d16fa78f" },
                { "gl", "33829dd7105a1a070f90c44f56be4b6d6be4d3e0f29f6b59435334cde815a849a8ace246d5070ba2314ddc20af7956906c608c0f8434f2aa1ce1eeffdb4fd76a" },
                { "he", "8ab3d99256a2105b8ceed987953635608ca449a1d5878090f2972ec0ad8ec96b5c84fb802d9c63b21fdb1c73097a6e1fcad8e2ae02d87ccd2e6029af038c3062" },
                { "hr", "87211c8dc590d787920f0dfddaea0cef0ac9bebc6eb7407968a1c6370995c2953fdfdc3800f527856ff9cac5176f3e756b44a7fec481b90bdb9da87853750beb" },
                { "hsb", "9b5a948772a5cf14724f56f7d1c35b7bd8a4a9b1625f8f4aefd50898c6ba09ccd499f9eda5f8949dbbf520085ca27ef46be69a955ca17e61a8e3cf3358abf9b0" },
                { "hu", "cd808ea1d58fc227caea8029cfe9fd0d648a15f3cb7812ca99b56525c50842c9ebc132a8a0c2b7dd9857be1f25972dd0fc222624a68e2dd3cc2c00e7a4afd7d2" },
                { "hy-AM", "d2a8ee713ffba271d901744c53258d9ddb0a441470a3744e995dd969524d1655a6d5debc3634bf71edba005a1b980a1e73e0b97503ebdfcd07178ca6a808eda4" },
                { "id", "ed23f8ab8d732dd3fbf255c6790520052d830324d3369fb7304a9d09d9d6974e900fb9419cd604f795b5e03f856fcef7751583e61b15cf68745e8942d8cdf81f" },
                { "is", "27d8a5e397178429c954d8f92fd05df123517474aed74f3e2f1a422d21c20ed7814114e243e9d6f7c91c16ab221768211a1ea6f8b778848b0ed5b458eeeab829" },
                { "it", "b8141a84ea6a7e6cd14e8d3c3b47b93525f0a57ecc98f948c3b4106315694ef379540af07d6a0349c333097a3a3a20dd9e46a5a24bacb77d6f28f41458cbff7d" },
                { "ja", "30b5ebeda8139429d82352ed824409b21bf19b5dc9fc5e277f946cbc9cfa4e55502e4785d9e6dd3fd9cb177019f7f8bd608d0210a55dd7c4d0e5e56acf3f640c" },
                { "ka", "9707fe13bd0d5d8d31ea61adbf0289f62fd76cd08575653fed46837f5986cb40648f4688237069af47573e7d91961ad5a098f006b84b78da23afeebcca64a772" },
                { "kab", "89ad6ed539a4429caf55016bf70241d476d752410b5597de600488bc978b7859509d7485efce5fc8b8b85b74faac2de37a66ac2bb66c29d9634542380774c1a1" },
                { "kk", "e598464ec3657bcef1d76ca900a62ebb08e7264af9cef8614d1d75717e8f5fb9605508460c40cba8a0ab895d40527b0a017991cc9f85cd6fe48623d244ce8490" },
                { "ko", "65168b903c1d630958f3e958f4d7d031d01ef1b6803840c1b86be4c3de46df554919e4fa3a2ee6ae139cb487069fcab8da1164db517290ff9be5a940c3fa1396" },
                { "lt", "616fe0af47d6d6c1f19311cf9a09498294f67fa93fccaea65e008ddb1c4e38513b2e04f9be5e9b83610b92f3d3aeaac8b0e9a5f8849ff0471a78e9d5f809a56a" },
                { "ms", "1821cc6fb3ec1acfdb9eba7904ddd65d40c307370d67866087c30bfe703bca1671dbdf99bf88866ea97739e9cf0b08b2cf8d6a952c5ee31be3a8f45b48f926d4" },
                { "nb-NO", "ada37bb6cfc24ecaa0797f143bab6bf795a02dac4a6b02ee34e2c3b1d83c453ff993adb8cecdf8ac65475caa67f6917f95a9d37b3bf2b0021c4a07b580c2352d" },
                { "nl", "3096ede562f53af1de5887fd7130e723caedd9396bd03138d495a8d0fc946e63d6ed8b6ccba65329636889be53cbc0676273f0b44fe7ca9632d5d40db9a42ad1" },
                { "nn-NO", "d9311ff702d3e4f7796e860a56a4fc38572ec8c081d1e28c55b9a075cf0368952926212f4d4ca30711bc74c1fd507d52fa1864369cb9e8584e3d6bc8bf161267" },
                { "pa-IN", "0d9cffd241756993c1575dd45661a0e86065d9bc27815815f2c454d767d3dc55ae0c30be497385305fa52f6a9d771a504b7fddaa0f4a85e138d44d6c8e4dc7d6" },
                { "pl", "0e14c74af0762d2900a8967ca1ee3369336a36828a96e82f0144b1e2a7fe47942a4fff4f354618174396cfdd3fec4853e86ab2798bbde8b0d2f10accd3c837f2" },
                { "pt-BR", "4c1d24f8bebe3b8d0b2d6619cd121da4b7dc98f9ec2ae8ece0614316338fbe1874ec6d14e1018329f050ea42d60624f47596763161b755fc23be5afca5e04342" },
                { "pt-PT", "adcfcb088c5ac4c76ba8e7411831370d37651ca8834bfe5c19f449d3227d1e85a4ad674c27285502c22cf29a2fc47846b4ec768c9dee7932e229cb3020d250bd" },
                { "rm", "08c8470c2cd1e69d832ae529f6b959adefd6a84fb8e88a760267d28f4ead95a7f9b5c9768d42b2802b8631a99c63b5f1f0e54a09bd230991425a2902a2dbaab6" },
                { "ro", "60ae8d62ecfbb2b74e108e9870b858477cd23c68bc8503d55d18cccee4245e0819fac8a0da9ec9a838bbd139589008e5235d5ab95624bbfe9d76aeaab01a08bd" },
                { "ru", "9be84c25270db44bf7342324959f16e2c9e1d7a22c98914516d87da870547773045f18be66669444fa8ffcb4f9fb4ef5a4513ecfe384302b179898982d0356d3" },
                { "si", "83be272c2e5f1c935af386ab72c75661308c7506f3334b8b5a3709322ca2028abda898f9b92fd27f32e46f7f4aa69c71597ab128889220871ad5b758dff221f0" },
                { "sk", "1e64499617c47e1fc095ce36d831b3c7c6ab5f665f90b2ec27a8db0918f52c24664cc344a2ae9a3f216e90ad9aba56d681e082530529f8eabfb410b222ebb1a9" },
                { "sl", "64fa8233410bd65ec2e3ee9e033670567bc477607b4e91d5a61be98708fc99bb89b0cb42b6da7d10be24ce03cdc6df7e999ec369fb913cd7355d96994fd010cc" },
                { "sq", "205c22cffa0ebc38f6621f0e163b2d1ea6ca9b6a0d77378f7e7f22fb5c243823a44ff0887208d11bbfdb499e4beee2f44b23922fce3a80a0dee703ac136894d4" },
                { "sr", "4975c5f45f5dfc91728ca1d532bd3f85552cdd3cfacb5e73dac7d701c2881a1fc790fd710ac825ab31489b5a7cc63ea4cd65bf1b8449ad26a036cd2cca2114dc" },
                { "sv-SE", "48a895488a47f5ed862cf471d6d5691efe8ee4b69de0ea70f353e8308146b732cfff8178bb16e0aec939d3d9f2fc8e5554cf120a7db9138ebc886ba41366ead7" },
                { "th", "244f5a85ef428190c23b558159d9acad18f2898f90d7aa74f3b6c9870f42e958546c89d637ea914cfe6139cd58e8e74d482909544506c4c2b8c738ac79eab37c" },
                { "tr", "18ee314ad3ca497d8378f20679895daeebd7095d7060063d00866669998c196dd6dee5fe723856d978f3af0409e2643e4d290f2f7e3c7cdf3d27fe0c4b3584d1" },
                { "uk", "a9c9a4d3eaaa3391b56c1ba71c4ab23ba4909161b1d952f9e00ea715eefa147b46987eee7c6265a7e0b754c37a69e7e1ae0575c91a1eff07ed2fe932f48b624e" },
                { "uz", "cb9043ccba641fdc82252b911e9f3578cbc5367b6a514176f21603db2a211a37347f69fb63b485e6ed4fbd6b9ded1119ed91a4c1a2de6534ee0b0de89cf5f079" },
                { "vi", "dc18fae61ae1e6f84f2fa8b17cced8bb9cfbb22fb98d304b05eac12f07cb197563702062f92f206dbb858b88cf303e6f902bf42ec71cf53fb8d2c073ba739893" },
                { "zh-CN", "da4b73cd4f6b95b197d57467b92ac8ff52a3e87e1b3cc001f482590971225ae5455149533fc328c3a009ec4cae1b306b59eaf45b84cd6abd2aef27a50a3748c8" },
                { "zh-TW", "603ca11e69d68c3736eee6a9ca59021959d672906367c01582169859296ff127c69fd10ba1d2fd70d924d8f36945a78037a8f312214ce8fc031659ee3f4fb821" }
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
            const string version = "78.13.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
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
